using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

class ContextFreeGrammar
{

  static Regex grammar = new Regex("(?<Symbol>[A-Z][A-Z0-9]*)\\s*[-=]?>(?<rule>\\s*'(?<terminal>[a-z0-9][a-z0-9]*)'\\s*\\|?|\\s*(?<nonterminal>[A-Z][A-Z]*)\\s*\\|?)*$");

  private List<NonTerminal> nonTerminals = new List<NonTerminal>();

  public void AddNonTerminal(NonTerminal nT) { nonTerminals.Add(nT); }

  public List<NonTerminal> GetNonTerminals() { return nonTerminals; }

  public ContextFreeGrammar(string file)
  {
    if (!File.Exists(file)) return;

    readGrammarFile(file);
    EliminateEpsilonRules();
    RemoveLeftRecursion();
    RemoveIndirectLeftRecursion();
  }

  private void EliminateEpsilonRules()
  {
    Boolean hasEpsilon = false;

    do
    {
      hasEpsilon = false;
      foreach (NonTerminal nT in nonTerminals.ToList())
      {
        for (int i = nT.GetProductions().Count - 1; i >= 0; i--)
        {
          if (nT.GetProductions()[i].Getwords().Count == 0)
          {
            nT.GetProductions().RemoveAt(i);
            hasEpsilon = true;
            AddNewNonEpsilonProductions(nT.symbol);
          }
        }
      }
    } while (hasEpsilon);

    Console.WriteLine("After Epsilon eliminiation:");
    Console.WriteLine(printGrammar());
  }

  private void AddNewNonEpsilonProductions(string symbol)
  {
    //Console.WriteLine("Remove from {0}", symbol);
    foreach (NonTerminal nT in nonTerminals.ToList<NonTerminal>())
    {

      List<Production> new_productions = new List<Production>();

      for (int j = nT.GetProductions().Count - 1; j >= 0; j--)
      {

        int occurences = 0;
        foreach (Word w in nT.GetProductions()[j].Getwords())
          if (w.value.Equals("(?&" + symbol + ")"))
            occurences++;

        //copy the production
        Production copy = nT.GetProductions()[j];

        //remove production
        nT.GetProductions().RemoveAt(j);

        //Amount of time we have to replace our production
        int numset = 1 << occurences;

        for (int i = 0; i < numset; i++)
        {
          int nth_nT = 0;
          Production new_production = new Production();

          foreach (Word w in copy.Getwords())
          {
            if (w.value.Equals("(?&" + symbol + ")"))
            {
              if (Convert.ToBoolean(i & (1 << nth_nT)))
                new_production.AddWord(w);
              nth_nT++;
            }
            else
            {
              new_production.AddWord(w);
            }
          }

          new_productions.Add(new_production);
        }
      }

      nT.AddRules(new_productions);
      //nT.GetProductions().AddRange(new_productions);

    }
  }

  private void RemoveLeftRecursion()
  {
    for (int i = nonTerminals.Count - 1; i >= 0; i--)
    {
      NonTerminal derived_nT = new NonTerminal(nonTerminals[i].symbol + i);

      for (int j = nonTerminals[i].GetProductions().Count - 1; j >= 0; j--)
      {
        Production p = nonTerminals[i].GetProductions()[j];

        if (!(p.Getwords().Count > 0 && p.Getwords()[0].value.Equals("(?&" + nonTerminals[i].symbol + ")")))
          continue;

        nonTerminals[i].GetProductions().RemoveAt(j);

        if (p.Getwords().Count == 1) continue;

        Production derived_prod = new Production();
        for (int p_i = 0; p_i < p.Getwords().Count; p_i++)
          if (p_i != 0)
            derived_prod.AddWord(w: p.Getwords()[p_i]);

        derived_prod.AddWord(new Word(derived_nT.symbol, false));

        derived_nT.AddRule(derived_prod);
      }

      if (derived_nT.GetProductions().Count > 0)
      {
        derived_nT.AddRule(new Production()); // Epsilon 

        Boolean hasDerivate = false;
        for (int d = 0; d < nonTerminals.Count; d++)
          if (nonTerminals[d].symbol.Equals(derived_nT.symbol))
          {
            Console.WriteLine("Add new prod rule to derivate " + nonTerminals[d].symbol);
            hasDerivate = true;
            nonTerminals[d].AddRules(derived_nT.GetProductions());
            addDerivateToProductions(nonTerminals[i].GetProductions(), nonTerminals[d]);
          }

        if (hasDerivate) continue;

        AddNonTerminal(derived_nT);
        addDerivateToProductions(nonTerminals[i].GetProductions(), derived_nT);
      }
    }

    Console.WriteLine("After Removing LeftRecursion:");
    Console.WriteLine(printGrammar());

    EliminateEpsilonRules();
  }

  private void addDerivateToProductions(List<Production> productions, NonTerminal derivate)
  {
    foreach (Production p in productions)
      p.AddWord(new Word(derivate.symbol, false));
  }

  private void RemoveIndirectLeftRecursion()
  {
    bool hasLeftRecursion = false;

    do
    {
      hasLeftRecursion = false;
      for (int i = 0; i < nonTerminals.Count; i++)
      {
        for (int j = nonTerminals[i].GetProductions().Count - 1; j >= 0; j--)
        {
          Production p = nonTerminals[i].GetProductions()[j];
          if (p.Getwords().Count == 0 || (p.Getwords().Count > 0 && p.Getwords()[0].terminal))
            continue;

          //search nonTerminal Production
          string symbol = p.Getwords()[0].value.Substring(3, p.Getwords()[0].value.Length - 4);
          //Console.WriteLine("Search symbol'{0}'", symbol);

          if (!symbol.Equals(nonTerminals[i].symbol))
            SearchGrammarForLeftRecursion(nonTerminals[i], symbol);
          else
            hasLeftRecursion = true;
        }
      }

      Console.WriteLine("After Removing indirect Left reucrsion:");
      Console.WriteLine(printGrammar());

      RemoveLeftRecursion();

    } while (hasLeftRecursion);

  }

  private void SearchGrammarForLeftRecursion(NonTerminal original_nT, string nTSymbol)
  {

    for (int i = nonTerminals.Count - 1; i >= 0; i--)
    {
      if (!nonTerminals[i].symbol.Equals(nTSymbol)) continue;

      for (int j = nonTerminals[i].GetProductions().Count - 1; j >= 0; j--)
      {
        Production p = nonTerminals[i].GetProductions()[j];

        if (p.Getwords().Count == 0 || (p.Getwords().Count > 0 && p.Getwords()[0].terminal))
          continue;

        string nextSymbol = p.Getwords()[0].value.Substring(3, p.Getwords()[0].value.Length - 4);
        if (!nextSymbol.Equals(original_nT.symbol))
        {
          if (!nextSymbol.Equals(nTSymbol))
            SearchGrammarForLeftRecursion(original_nT, nextSymbol);
          return;
        }

        //Replace the indirect reucrsion
        Production tail_prod = new Production();
        for (int p_i = 0; p_i < p.Getwords().Count; p_i++)
          if (p_i != 0) tail_prod.AddWord(p.Getwords()[p_i]);

        //Remove original prod
        nonTerminals[i].GetProductions().RemoveAt(j);

        //Add all new production
        foreach (Production original_prod in original_nT.GetProductions().ToList())
        {

          Production new_prod = new Production();
          foreach (Word w in original_prod.Getwords())
            new_prod.AddWord(w);
          foreach (Word w in tail_prod.Getwords())
            new_prod.AddWord(w);

          nonTerminals[i].AddRule(new_prod);
        }
      }
    }
  }

  public void readGrammarFile(string file)
  {
    string[] grammarDefinition = File.ReadAllLines(file);

    for (int i = 0; i < grammarDefinition.Length; i++)
    {

      Console.WriteLine("In {0}", grammarDefinition[i]);

      Match match = grammar.Match(grammarDefinition[i]);

      if (!match.Success) throw new Exception(String.Format("Couldn't read grammar definition in line {0}: {1}", (i + 1), grammarDefinition[i]));

      CaptureCollection words = match.Groups["rule"].Captures;

      string symbol = match.Groups["Symbol"].Value;
      NonTerminal nT = new NonTerminal(symbol);

      Production prodRule = new Production();

      foreach (Capture word in words)
      {
        if (word.Value.Contains("'") && !word.Value.Contains("|"))
        {
          string terminal = word.Value.Replace("'", "");
          terminal = terminal.Replace(" ", "");
          prodRule.AddWord(new Word(terminal, true));
        }
        if (!word.Value.Contains("'") && !word.Value.Contains("|"))
        {
          string nonTerminal = word.Value.Replace(" ", "");
          prodRule.AddWord(new Word(nonTerminal, false));
        }
        if (word.Value.Contains("'") && word.Value.Contains("|"))
        {
          string terminal = word.Value.Replace("'", "");
          terminal = terminal.Replace(" ", "");
          terminal = terminal.Replace("|", "");
          prodRule.AddWord(new Word(terminal, true));
          nT.AddRule(prodRule);
          prodRule = new Production();
        }
        else if (word.Value.Contains("|"))
        {
          string nonTerminal = word.Value.Replace(" ", "");
          nonTerminal = nonTerminal.Replace("|", "");
          prodRule.AddWord(new Word(nonTerminal, false));
          nT.AddRule(prodRule);
          prodRule = new Production();
        }
      }

      nT.AddRule(prodRule);

      this.AddNonTerminal(nT);
    }
  }

  public string createPCREString()
  {

    string start = "(?(DEFINE)";

    nonTerminals.ForEach(nt => start += nt.CreateNonTerminalString());

    start += ")";

    if (nonTerminals.Count == 0) return start;

    start += "^(?&" + nonTerminals[0].symbol + ")$";

    return start;
  }

  public string printGrammar()
  {
    string grammar = "";

    nonTerminals.ForEach(nt => grammar += nt.CreateNonTerminalGrammar());

    return grammar;
  }

}