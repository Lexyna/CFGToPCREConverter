using System;
class Word
{
  public string value { get; set; }

  public string symbol { get; private set; }

  public Boolean terminal { get; private set; }

  public Word(string value, Boolean terminal)
  {
    this.value = terminal ? value : "(?&" + value + ")";
    this.symbol = terminal ? "'" + value + "'" : value;
    this.terminal = terminal;
  }
}