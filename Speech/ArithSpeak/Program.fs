open System
open System.Speech.Recognition
open System.Speech.Recognition.SrgsGrammar

let sr_SpeechRecognized sender (e:SpeechRecognizedEventArgs)  =
    printfn "Confidence: %s" (e.Result.Confidence.ToString())
    printfn "Text: %s" e.Result.Text

[<EntryPoint>]
let main argv =
    let document = Language.createGrammarDocument

    // Create a Grammar object, initializing it with the root rule.
    let g = new Grammar(document, document.Root.Id)

    // Create an in-process speech recognizer for the en-US locale.  
    use recognizer =  new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"))

    // Load the Grammar object into the recognizer.
    recognizer.LoadGrammar(g);

    // Configure input to the speech recognizer.  
    recognizer.SetInputToDefaultAudioDevice();  
  
    // Start asynchronous, continuous speech recognition.  
    recognizer.RecognizeAsync(RecognizeMode.Multiple);  

    // Produce an XML file that contains the grammar.
    let writer = System.Xml.XmlWriter.Create("srgsDocument.xml")
    document.WriteSrgs(writer)
    writer.Close()

    recognizer.SpeechRecognized.AddHandler(new EventHandler<SpeechRecognizedEventArgs>(sr_SpeechRecognized))
    while (true) do
        Console.Read() |> ignore
    0 // return an integer exit code
