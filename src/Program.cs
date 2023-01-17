using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


class Program
{
  static Regex grammar = new Regex("(?<Symbol>[A-Z][A-Z0-9]*)\\s*[-=]?>(?<rule>\\s*'(?<terminal>[a-z][a-z0-9]*)'\\|?|\\s*(?<nonterminal>[A-Z][A-Z]*)\\s*\\|?)*$");

  static void Main(string[] args)
  {

    if (args.Length == 0) return;

    ContextFreeGrammar cfg = new ContextFreeGrammar(args[0]);

    Console.WriteLine(cfg.createPCREString());

    /*if (!File.Exists(args[0]))
    {
      Console.WriteLine("Couldn't find {0}", args[0]);
      return;
    }

    ContextFreeGrammar cfg = readGrammarFile(args[0]);

    EliminateLeftRecursion(cfg);

    string pcreSring = cfg.createPCREString();

    Console.WriteLine(pcreSring);*/
  }

  /*private static ContextFreeGrammar readGrammarFile(string file)
  {

    ContextFreeGrammar cfg = new ContextFreeGrammar();

    string[] grammarDefinition = File.ReadAllLines(file);

    for (int i = 0; i < grammarDefinition.Length; i++)
    {

      Console.WriteLine("In {0}", grammarDefinition[i]);

      Match match = grammar.Match(grammarDefinition[i]);

      CaptureCollection words = match.Groups["rule"].Captures;

      string symbol = match.Groups["Symbol"].Value;
      NonTerminal nT = new NonTerminal(symbol);

      ProductionRule prodRule = new ProductionRule();

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
          prodRule = new ProductionRule();
        }
        else if (word.Value.Contains("|"))
        {
          string nonTerminal = word.Value.Replace(" ", "");
          nonTerminal = nonTerminal.Replace("|", "");
          prodRule.AddWord(new Word(nonTerminal, false));
          nT.AddRule(prodRule);
          prodRule = new ProductionRule();
        }
      }

      nT.AddRule(prodRule);

      cfg.AddNonterminal(nT);
    }

    return cfg;
  }

  private static void EliminateLeftRecursion(ContextFreeGrammar cfg)
  {

    //Convert all Nonterminals to A1,...,AN
    List<NonTerminal> nT = cfg.getNonTerminals();

    for (int i = 0; i < nT.Count; i++)
    {

      string originalSymbol = nT[i].symbol;
      nT[i].symbol = "A" + i;

      for (int j = 0; j < nT.Count; j++)
      {
        foreach (ProductionRule rule in cfg.getNonTerminals()[j].getProductionRules())
        {
          foreach (Word w in rule.getWords())
          {
            if (!w.terminal && w.value.Equals("(?&" + originalSymbol + ")"))
              w.value = w.value.Replace(originalSymbol, "A" + i);
          }
        }
      }



    }
  }

  private class ContextFreeGrammar
  {
    private List<NonTerminal> nonTerminals = new List<NonTerminal>();

    public void AddNonterminal(NonTerminal nT)
    {
      nonTerminals.Add(nT);
    }

    public List<NonTerminal> getNonTerminals()
    {
      return nonTerminals;
    }

    public string createPCREString()
    {

      string start = "(?(DEFINE)";

      nonTerminals.ForEach(nt => start += nt.CreateNTRule());

      start += ")";

      if (nonTerminals.Count == 0) return start;

      start += "^(?&" + nonTerminals[0].symbol + ")$";

      return start;
    }

  }

  private class NonTerminal
  {
    public string symbol { get; set; }
    private List<ProductionRule> rules = new List<ProductionRule>();

    public NonTerminal(string symbol)
    {
      this.symbol = symbol;
    }

    public void AddRule(ProductionRule rule) { rules.Add(rule); }

    public List<ProductionRule> getProductionRules()
    {
      return rules;
    }

    public string CreateNTRule()
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

  }

  private class ProductionRule
  {
    private List<Word> words = new List<Word>();

    public void AddWord(Word w) { words.Add(w); }

    public List<Word> getWords() { return words; }

    public string GetProductionString()
    {
      string production = "";

      words.ForEach(w => production += w.value);

      return production;
    }

  }
  private class Word
  {
    public string value { get; set; }
    public Boolean terminal { get; private set; }

    public Word(string value, Boolean terminal)
    {
      this.value = terminal ? value : "(?&" + value + ")";
      this.terminal = terminal;
    }
  }*/

}