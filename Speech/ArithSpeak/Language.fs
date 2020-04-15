module Language
open FParsec

type Operation = Push of int | Add | Subtract | Multiply | Call of Definition
and Definition = Define of (string * Operation list) 


type OperationParser = Parser<Operation,unit>
let pPush : OperationParser = pstring "push " >>. pint32 >>= fun x -> fun s -> Reply (Push x)
let pAdd : OperationParser = stringReturn "add" Add
let pSubtract : OperationParser = stringReturn "subtract" Subtract
let pMultiply : OperationParser = stringReturn "multiply" Multiply
let pOperation : OperationParser =
    choice [pPush; pAdd; pSubtract; pMultiply]

let runParser str =
    match run pOperation str with
    | Success(result, _, _)   -> Some result
    | Failure(errorMsg, _, _) -> 
        printfn "Failure: %s" errorMsg
        None
