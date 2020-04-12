using System;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;

namespace Tutorial
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Create a new SpeechRecognizer instance.
            SpeechRecognizer sr = new SpeechRecognizer();

            // Create the "Subject" rule.
            SrgsRule subjRule = new SrgsRule("id_Subject");
            SrgsOneOf subjList = new SrgsOneOf(new string[] { "I", "you", "he", "she", "Tom", "Mary" });
            subjRule.Add(subjList);

            // Create the "Verb" rule.
            SrgsRule verbRule = new SrgsRule("id_Verb");
            SrgsOneOf verbList = new SrgsOneOf(new string[] { "ate", "bought", "saw", "sold", "wanted" });
            verbRule.Add(verbList);

            // Create the "Object" rule.
            SrgsRule objRule = new SrgsRule("id_Object");
            SrgsOneOf objList = new SrgsOneOf(new string[] { "apple", "banana", "pear", "peach", "melon" });
            objRule.Add(objList);

            // Create the root rule.
            // In this grammar, the root rule contains references to the other three rules.
            SrgsRule rootRule = new SrgsRule("Subj_Verb_Obj");
            rootRule.Scope = SrgsRuleScope.Public;

            // Create the "Subject" and "Verb" rule references and add them to the SrgsDocument.
            SrgsRuleRef subjRef = new SrgsRuleRef(subjRule, "theSubject");
            rootRule.Add(subjRef);

            SrgsRuleRef verbRef = new SrgsRuleRef(verbRule, "theVerb");
            rootRule.Add(verbRef);

            // Add logic to handle articles: "the", "a", "and", occurring zero or one time.
            SrgsOneOf articles = new SrgsOneOf(
                new SrgsItem[] { new SrgsItem(0, 1, "the"), new SrgsItem(0, 1, "a"), new SrgsItem(0, 1, "an") }
            );
            rootRule.Add(articles);

            // Create the "Object" rule reference and add it to the SrgsDocument.
            SrgsRuleRef objRef = new SrgsRuleRef(objRule, "theObject");
            rootRule.Add(objRef);

            // Create an SrgsDocument object that contains all four rules.
            SrgsDocument document = new SrgsDocument();
            document.Rules.Add(new SrgsRule[] { rootRule, subjRule, verbRule, objRule });

            // Set "rootRule" as the root rule of the grammar.
            document.Root = rootRule;

            // Create a Grammar object, initializing it with the root rule.
            Grammar g = new Grammar(document, "Subj_Verb_Obj");

            // Load the Grammar object into the recognizer.
            sr.LoadGrammar(g);

            // Produce an XML file that contains the grammar.
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create("srgsDocument.xml");
            document.WriteSrgs(writer);
            writer.Close();

            sr.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_SpeechRecognized);
            while (true)
                Console.Read();
        }

        static void sr_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine($"Confidence: {e.Result.Confidence}");
            Console.WriteLine($"Text: {e.Result.Text}");
            Console.WriteLine($"Verb: {e.Result.Semantics["theVerb"].Value}");
            Console.WriteLine($"Value: {e.Result.Semantics.Value}");
        }
    }
}
