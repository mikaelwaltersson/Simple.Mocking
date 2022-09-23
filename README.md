# Simple.Mocking

Easy to use framework for creating mock objects, fakes, stubs with an intuitive and fluent API

* Can mock/fake any interface or delegate
* Refactor friendly syntax, no "magic string" (except for event add/remove)
* No record/replay which just clutters down test code
* Easy to learn, small API

**.NET Core version (2.0) released at September 23, 2022**  
No feature changes, just upgraded to .NET Core 6.0 and enabled the `Nullable` language feature.


**.NET Core version (2.0) released at September 2, 2019**  
No feature changes, just ported to .NET Core 2.0. (Thanks to pull request from [@rerikson](https://github.com/rerikson))


**Other sources of information**

* Filip Ekberg has written a blog post about mocking in .NET using this framework:      
  http://blog.filipekberg.se/2012/01/30/getting-started-with-a-mocking-framework/


## Getting started with Simple.Mocking

Short guide on how to use the Simple.Mocking framework. 
There are just six classes to keep in mind when using the public API of the framework.

* `Mock` for creating mock instances
* `Expect` for specifying expected invocations with optional actions on mock instances
* `Any<T>` for specifying wildcard parameter values for expected method invocations
* `ExpectationScope` for specifying expected behaviour containing multiple mock instances and ordered invocations
* `AssertInvocationsWasMade` for verifying that expectations has been met for a mock instance or expectation scope
* `Stub`  convenience methods for setting up mocks as stubs



### Create mock for an interface or delegate


```csharp 
  
  var myObject = Mock.Interface<IMyObject>();
  var myDelegate = Mock.Interface<MyDelegate>();
  
```

It is an active choice not to implement mocking of abstract and concrete classes, to promote a "code against interfaces, not implementations" development style. Mocking parts of an implementation, which is what you do when you mock specific methods of an abstract or concrete class adds complexity and is not as clear as mocking an interface. I you really have to mock an abstract class, I recommend doing it manually.

As for delegates, they are just single method interfaces.



###  Specifying expected invocations

```csharp 
  
  Expect.MethodCall(() => myObject.MyMethod(11, out Any<string>.Value.AsRefOrOut));

  Expect.MethodCall(() => myDelegate(Any<int>.Value.Matching(i => i > 0 && < 10));

  Expect.PropertyGet(() => myObject.MyProperty);
  Expect.PropertyGet(() => myObject[2, 3]);

  Expect.PropertySet(() => myObject.MyProperty, 42);
  Expect.PropertySet(() => myObject[2, 3], 42);

  Expect.EventAdd(myObject, "MyEvent", Any<EventHandler>.Value);
  Expect.EventRemove(myObject, "MyEvent", Any<EventHandler>.Value)
  
```


The not so ideal way to mock property set and event add/remove comes from inability to specify expressions in C# containing property or event assignments.

An unexpexted invocation will throw an `ExpectationsException` by default so your tests fail fast, which is a god thing. The exception to this rule is the three methods declared in `System.Object`, `ToString()`, `Equals(object obj)` and `GetHashCode()`. This methods has default implementations but can still be overriden with mocked expectations.

Just think of an mock instance without expectations as an class implementing the mocked interface with method bodies containing `throw new ExpectationsException();`. It will still have the `System.Object` methods implemented. 

If you need to have default implementations of all methods for on an mock instance you can still do:
```csharp
  Expect.AnyInvocationOn(myObject);
```
(the default implementations will always set return value and out parameter to the default value for the type.)


### Using "wildcard" parameter values
```csharp

  // Matches any value of that can be assigned to an parameter of type T
  Any<T>.Value 

  // Only matches any T values which matches a certain predicate
  Any<T>.Value.Matching(a => Predicate(a))

  // For use as ref or out parameter values
  Any<T>.Value.AsOutOrRef
  Any<T>.Value.Matching(a => Predicate(a)) .AsOutOrRef

```

### Limiting number of expected invocations
```csharp

  Expect.AtLeastOnce.MethodCall(() => myObject.MyMethod());
  Expect.AtLeast(4).MethodCall(() => myObject.MyMethod());
  
  Expect.Once.MethodCall(() => myObject.MyMethod());
  Expect.Exactly(4).MethodCall(() => myObject.MyMethod());
  
  Expect.AtMostOnce.MethodCall(() => myObject.MyMethod());
  Expect.AtMost(4).MethodCall(() => myObject.MyMethod());
  
  Expect.Between(2, 4).MethodCall(() => myObject.MyMethod());

```

All limits are inclusive and the lower limit only has any real effect in combination with `AssertExpectations.IsMetFor` and/or `ExpectationScope.BeginOrdered`.



### Specifying actions and return values

```csharp

  Expect.MethodCall(() => myObject.MyMethod()).Executes(() => DoSomeThing());

  Expect.MethodCall(() => myObject.MyMethod(ref Any<int>.Value.AsOutOrRef, Any<int>.Value)).Executes(parameters => (int)parameters[0] = (int)parameters[1] * 2);

  Expect.MethodCall(() => myObject.MyMethod()).Returns(27);

  Expect.MethodCall(() => myObject.MyMethod()).Throws(new Exception("ERROR"));

  Expect.MethodCall(() => myObject.MyMethod(ref Any<int>.Value.AsOutOrRef, 6)).SetsOutOrRefParameter(0, 12).Returns(12);

```

### Expectation scopes
```csharp

  var expectationScope = new ExpectationScope();

  var myObject1 = Mock.Interface<IMyObject>(expectationScope);
  var myObject2 = Mock.Interface<IMyObject>(expectationScope);

  using (expectationScope.BeginOrdered())
  {
    using (expectationScope.BeginUnordered())
    {
      Expect.Once.MethodCall(() => myObject1.Start());			
      Expect.Once.MethodCall(() => myObject2.Start());
    }

    using (expectationScope.BeginUnordered())
    {     
      Expect.AtLeastOnce.MethodCall(() => myObject1.DoWork(Any<string>.Value));
      Expect.AtLeastOnce.MethodCall(() => myObject2.DoWork(Any<string>.Value));
    }

    using (expectationScope.BeginUnordered())
    {           
      Expect.Once.MethodCall(() => myObject1.Stop());			
      Expect.Once.MethodCall(() => myObject2.Stop());
    }
  }

```

Each mock object has an expectation scope which contains all expected invocations. When a mock object is created an exception scope is assigned to it. (calling `Mock.Interface<IMyObject>()` is the same as calling `Mock.Interface<IMyObject>(new ExpectationScope())`

Controlling the assignment of expectations scopes for mock objects can be used when certain method calls on one or more objects is expected to be called in a certain order or to if you want to be able to verify expectations for several mock objects with one `AssertExpectations.IsMetFor` call.

The root scope is always unordered, `BeginUnordered()` and `BeginOrdered()` can be used to specify unordered/ordered child scopes.



### Assert that expectations has been met

```csharp

AssertInvocationsWasMade.MatchingExpecationsFor(myObject);

AssertInvocationsWasMade.MatchingExpecationsFor(expectationScope);

```
Calling `AssertInvocationsWasMade.MatchingExpecationsFor` on a mock object will verify that all expectations has been met in the whole expectation scope assigned to the object, so if two mock objects share one expectation scope calling `AssertInvocationsWasMade.MatchingExpecationsFor` on one of the mock objects will verify expectations for both objects.

For example, the following code:
```csharp

  var expectationScope = new ExpectationScope();
  var myObject1 = Mock.Interface<IMyObject>(expectationScope);
  var myObject2 = Mock.Interface<IMyObject>(expectationScope);

```

Makes the following three method calls equivalent to each other:

```csharp
  
  AssertInvocationsWasMade.MatchingExpecationsFor(myObject1);
  AssertInvocationsWasMade.MatchingExpecationsFor(myObject2);
  AssertInvocationsWasMade.MatchingExpecationsFor(expectationScope);
  
```

"AAA"-style verification of expected method calls, useful for mocks of observer style objects
```csharp

  AssertInvocationsWasMade.
    Once.ForMethodCall(() => myObject.DoWork("file1.txt")).
    Once.ForMethodCall(() => myObject.DoWork("file2.txt")).
    InOrderAsSpecified();
      
```


### Stubs

`Stub.Interface` (and `Stub.Delegate`) is equivalent to:

```csharp

  var myObject = Mock.Interface<IMyObject>();

  Expect.AnyInvocationOn(myObject).
    SetsOutOrRefParameters(StubValue.ForType).
    Returns(StubValue.ForType);
      
```

Where `StubValue.ForType` is a static method which will return a default stub value for any given type.

* For interfaces and delegate types a new stubbed instances will be returned
* For concrete class types with public default constructors a new instance will be returned
* For the type string empty string will be returned
* For all array types empty arrays will be returned
* For value types default value will be returned
* For all other types `null` will be returned


`Stub.MethodCall` and `Stub.PropertyGet` is short versions of:

```csharp

  Expect.WithHigherPrecedence.MethodCall(() => myObject.Method());

  Expect.WithHigherPrecedence.PropertyGet((() => myObject.Property);

```
 
 
## Contributors
* [@mikaelwaltersson](https://github.com/mikaelwaltersson)
* [@rerikson](https://github.com/rerikson)
  
