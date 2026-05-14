namespace TransactionSystemApi.Models;

public class Card
{
    public Guid Id { get; private set; }
    
    public decimal Balance { get; set; }
    
    // Not in the requirement but would be good to add in reality
    // public string FirstName { get; set; }
    //
    // public string LastName { get; set; }
    //
    // public int CardNumber { get; set; }
    //
    // public int Cvv { get; set; }
}