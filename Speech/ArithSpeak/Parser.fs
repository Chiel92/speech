module Parser
open Language
open FParsec

type OperationParser = Parser<Operation,unit>
let pPush : OperationParser = pstring "push " >>. pint32 >>= fun x -> fun s -> Reply (Push x)
let pAdd : OperationParser = stringReturn "add up" Add
let pSubtract : OperationParser = stringReturn "subtract" Subtract
let pMultiply : OperationParser = stringReturn "multiply" Multiply
let pSwap : OperationParser = stringReturn "swap" Swap
let pRotate : OperationParser = stringReturn "rotate" Rotate
let pDuplicate : OperationParser = stringReturn "duplicate" Duplicate
let pScratch : OperationParser = stringReturn "scratch" Scratch
let pOperation : OperationParser =
    choice [pPush; pAdd; pSubtract; pMultiply; pSwap; pRotate; pDuplicate; pScratch]

let runParser str =
    match run pOperation str with
    | Success(result, _, _)   -> Some result
    | Failure(errorMsg, _, _) -> 
        printfn "Failure: %s" errorMsg
        None
