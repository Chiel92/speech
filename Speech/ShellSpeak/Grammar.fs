module Grammar

open System.Speech.Recognition.SrgsGrammar


let createGrammarDocument =
    // Create the "Number" rule.
    let numberRule = new SrgsRule("id_number")

    let numbers =
        [ 0 .. 100 ] |> List.map (fun s -> s.ToString()) |> List.toArray

    let numberList = new SrgsOneOf(numbers)
    numberRule.Add(numberList)

    let letterRule = new SrgsRule("id_letter")

    let letters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray() |> Array.map (fun s -> s.ToString())

    let letterList = new SrgsOneOf(letters)
    letterRule.Add(letterList)

    let babbleRule = new SrgsRule("id_babble")
    let babbleList = new SrgsOneOf("uhm")
    babbleRule.Add(babbleList)

    // Create the "Command" rule.
    let cmdRule = new SrgsRule("id_cmd")
    let cmdList = new SrgsOneOf()
    //cmdList.Add(new SrgsItem(new SrgsRuleRef(babbleRule)))
    cmdList.Add(new SrgsItem("git status"))
    cmdRule.Add(cmdList)

    let rootRule = new SrgsRule("id_root")
    let rootList = new SrgsOneOf()
    rootList.Add(new SrgsItem(new SrgsRuleRef(cmdRule)))
    rootRule.Add(rootList)

    // Create an SrgsDocument object that contains all rules.
    let document = new SrgsDocument()
    document.Rules.Add(rootRule, cmdRule, numberRule, letterRule, babbleRule)

    // Set "rootRule" as the root rule of the grammar.
    document.Root <- rootRule

    document
