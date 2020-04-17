module Parser
open Language
open FParsec

// Parse a string if it ends with a word boundary, ignoring prefixed whitespace
let pWord word = regex (word + "\\b")
let wordReturn word result = pWord word >>. preturn result

let pBabble = spaces >>. (many (spaces >>. pWord "uhm")) >>. spaces

type OperationParser = Parser<Operation,unit>
let pPush : OperationParser = pWord "push" >>. spaces >>. pint32 >>= fun x -> preturn (Push x)
let pAdd : OperationParser = wordReturn "add together" Add
let pSubtract : OperationParser = wordReturn "subtract" Subtract
let pMultiply : OperationParser = wordReturn "multiply" Multiply
let pSwap : OperationParser = wordReturn "swap" Swap
let pRotate : OperationParser = wordReturn "rotate" Rotate
let pDuplicate : OperationParser = wordReturn "duplicate" Duplicate
let pScratch : OperationParser = wordReturn "scratch" Scratch
let pOperation : OperationParser =
    pBabble >>. choice [pPush; pAdd; pSubtract; pMultiply; pSwap; pRotate; pDuplicate; pScratch]

type DefinitionParser = Parser<Definition,unit>
let pDefinition : DefinitionParser =
    pWord "define" >>. pBabble >>. anyChar >>= fun name ->
        (pBabble >>. pWord "as" >>. many1 pOperation >>= fun operations ->
            preturn (Define (name.ToString(), operations)))

type CommandParser = Parser<Command,unit>
let pUndo : CommandParser = wordReturn "undo" Undo

let pRoot : CommandParser = choice  [
    pOperation >>= fun x -> preturn (Op x);
    pDefinition >>= fun x -> preturn (Def x);
    pUndo;
    ]

let runParser str =
    match run pRoot str with
    | Success(result, _, _)   -> Some result
    | Failure(errorMsg, _, _) -> 
        printfn "Parsing failure: %s" errorMsg
        None
