namespace Solution.Interfaces;

public interface IBanking
{
    int Balance { get; }
    void AddMoney(int amount);
    void RemoveMoney(int amount);
    void ShowAmount();
}
