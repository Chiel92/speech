module Language
open System
open System.Text
open System.Speech.Recognition
open System.Speech.Recognition.SrgsGrammar

type Operation = Push of int | Add | Subtract | Multiply | Call of Definition
and Definition = Define of (string * Operation list) 


let createGrammarDocument = 
    // Create the "Number" rule.
    let numberRule = new SrgsRule("id_number")
    let numbers = [1..10] |> List.map (fun s -> s.ToString()) |> List.toArray
    let numberList = new SrgsOneOf(numbers)
    numberRule.Add(numberList)

    let letterRule = new SrgsRule("id_letter")
    let letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray() |> Array.map (fun s -> s.ToString())
    let letterList = new SrgsOneOf(letters)
    letterRule.Add(letterList)

    // Create the "Operation" rule.
    let opRule = new SrgsRule("id_op")
    let opList = new SrgsOneOf()
    let pushItem = new SrgsItem()
    pushItem.Add(new SrgsItem("push"))
    pushItem.Add(new SrgsRuleRef(numberRule))
    opList.Add(pushItem)
    opList.Add(new SrgsItem("add"))
    opList.Add(new SrgsItem("subtract"))
    opList.Add(new SrgsItem("multiply"))
    let callItem = new SrgsItem()
    callItem.Add(new SrgsItem("call"))
    callItem.Add(new SrgsRuleRef(letterRule))
    callItem.Add(new SrgsItem("now"))
    opList.Add(callItem)
    opRule.Add(opList)

    // Create an SrgsDocument object that contains all rules.
    let document = new SrgsDocument();
    document.Rules.Add(opRule, numberRule, letterRule);

    // Set "rootRule" as the root rule of the grammar.
    document.Root <- opRule;

    document
