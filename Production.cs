using System.Collections;
using System.Collections.Generic;

class Production
{
  private List<Word> words = new List<Word>();

  public void AddWord(Word w) { words.Add(w); }

  public List<Word> Getwords() { return words; }

  public string GetProductionString()
  {
    string production = "";

    words.ForEach(w => production += w.value);

    return production;
  }

  public string GetProductionGrammar()
  {
    string producation = "";

    words.ForEach(w => producation += w.symbol + " ");

    return producation;
  }
}