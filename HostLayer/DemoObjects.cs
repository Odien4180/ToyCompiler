using System;

[ScriptExpose]
public class Player
{
    [ScriptExpose]
    public int Health { get; set; } = 50;

    [ScriptExpose]
    public int Score;

    [ScriptExpose]
    public int Counter;

    [ScriptExpose]
    public void Heal(int amount)
    {
        Health += amount;
    }

    [ScriptExpose]
    public int Add(int a, int b)
    {
        return a + b;
    }

    [ScriptExpose]
    public void PrintStatus()
    {
        Console.WriteLine($"[Player] Health={Health}, Score={Score}");
    }
}
