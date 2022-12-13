using System.Collections;
using System.Collections.Generic;
using System;

class NonTerminal
{

  public string symbol { get; set; }

  private List<Production> rules = new List<Production>();

  public NonTerminal(string symbol)
  {
    this.symbol = symbol;
  }

  public void AddRule(Production p) { rules.Add(p); }

  public void AddRules(List<Production> productions)
  {
    productions.ForEach(p =>
    {

      Boolean hasProduction = false;

      rules.ForEach(r =>
      {
        if (r.GetProductionGrammar().Equals(p.GetProductionGrammar()))
          hasProduction = true;
      });

      if (!hasProduction)
        AddRule(p);

    });
  }

  public List<Production> GetProductions() { return rules; }

  public string CreateNonTerminalString()
  {
    string pattern = "(?<" + symbol + ">";

    for (int i = 0; i < rules.Count; i++)
    {
      pattern += rules[i].GetProductionString();
      pattern += i == rules.Count - 1 ? "" : "|";
    }

    pattern += ")";

    return pattern;
  }

  public string CreateNonTerminalGrammar()
  {
    string pattern = symbol + " -> ";

    for (int i = 0; i < rules.Count; i++)
    {
      pattern += rules[i].GetProductionGrammar();
      pattern += i == rules.Count - 1 ? "" : "|";
    }

    pattern += "\n";

    return pattern;
  }

}