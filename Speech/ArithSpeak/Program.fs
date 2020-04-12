// Learn more about F# at http://fsharp.org

open System
open System.Speech.Recognition
open System.Speech.Recognition.SrgsGrammar

let sr_SpeechRecognized sender (e:SpeechRecognizedEventArgs)  =
    printf "Confidence: %s" (e.Result.Confidence.ToString())
    printf "Text: %s" e.Result.Text
    printf "Verb: %s" (e.Result.Semantics.["theVerb"].Value.ToString())

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let sr = new SpeechRecognizer()

    // Create an SrgsDocument object that contains all four rules.
    let document = new SrgsDocument();
    //document.Rules.Add(new SrgsRule[] { rootRule, subjRule, verbRule, objRule });

    // Set "rootRule" as the root rule of the grammar.
    //document.Root = rootRule;

    // Create a Grammar object, initializing it with the root rule.
    let g = new Grammar(document, "Subj_Verb_Obj")

    // Load the Grammar object into the recognizer.
    sr.LoadGrammar(g);

    // Produce an XML file that contains the grammar.
    let writer = System.Xml.XmlWriter.Create("srgsDocument.xml")
    document.WriteSrgs(writer);
    writer.Close();

    sr.SpeechRecognized.AddHandler(new EventHandler<SpeechRecognizedEventArgs>(sr_SpeechRecognized));
    while (true) do
        Console.Read() |> ignore
    0 // return an integer exit code
