# MockLight
MockLight is a light weight C# class library that enables mocking of interfaces to aid unit testing an can be used in a Linux environment.

## Creating a mock class implementation from an interface
```cs
// Interface to mock
public interface IAccount
{
    bool PayBill(int amount);
    Task<Statement> GetStatementAsync();
    void Verify();
}
// Mock class
public class MockAccount : Mock, IAccount
{
    // Getters
    public Person AccountHolder => Mocks.AccountHolder;

    // Methods from interface
    public bool PayBill(int amount)
    {
        return Mocks.PayBill(amount);
    }
    public Task<Statement> GetStatementAsync()
    {
        return Mocks.GetStatementAsync();
    }
    public void Verify()
    {
        Mocks.Verify();
    }
}
```
## Changing a mock method / getter implementation
Below is a demo of how to change mocked class methods and getters. This shows how to use the various overloads of the Mock class Setup method.
```cs
public class TestClassMocking
{
    private MockAccount mockAccount = new MockAccount();

    public void TestPayBillMethod()
    {
        // Arrange
        bool mockResult = true;
        mockAccount.MockSetup<int, bool>("PayBill", (amount) =>
        {
            return mockResult;
        });
    }

    public void TestAccountHolderMethod_One()
    {
        // Arrange
        Person mockResult = new Person();
        mockAccount.MockSetup<Person>("AccountHolder", () =>
        {
            return mockResult;
        });
    }

    public void TestAccountHolderMethod_Two()
    {
        // Arrange
        Person mockResult = new Person();
        mockAccount.MockSetup<Person>("AccountHolder", mockResult);
    }

    // Advanced setup
    public void TestAccountHolderMethod_Advanced()
    {
        // Arrange
        Person mockResult = new Person();
        mockAccount.MockSetup((mocks) =>
        {
            var name = "AccountHolder";
            mocks[name] = mockResult;
            // Manually call UpdateCalls method
            mockAccount.MockUpdateCalls(name);
        });
    }
}
```
## Verifying mocked objects
Below is a demo of how to verify mocked methods and getters.
```cs
public class TestClassVerifying
{
    private MockAccount mockAccount = new MockAccount();

    public void TestPayBillMethod()
    {
        // Arrange
        bool mockResult = true;
        int argument = 1;
        mockAccount.MockSetup<int, bool>("PayBill", (amount) =>
        {
            return mockResult;
        });
        // Act
        var result = mockAccount.PayBill(argument);
        // Assert
        var verify = mockAccount.MockVerify("PayBill");
        Assert.IsTrue(verify.HasBeenCalledTimes(1));
    }
}
```
