module Parser
open Language
open FParsec

// Parse a string if it ends with a word boundary
let pWord word = regex (word + "\\b")
let wordReturn word result = pWord word >>. preturn result

let ignoreBabble = spaces >>. (many (pWord "uhm" >>. spaces))

type OperationParser = Parser<Operation,unit>
let pPush : OperationParser = pWord "push" >>. spaces >>. pint32 >>= fun x -> preturn (Push x)
let pPull : OperationParser = pWord "pull" >>. spaces >>. pint32 >>= fun x -> preturn (Pull x)
let pAdd : OperationParser = wordReturn "add together" Add
let pSubtract : OperationParser = wordReturn "subtract" Subtract
let pMultiply : OperationParser = wordReturn "multiply" Multiply
let pDuplicate : OperationParser = wordReturn "duplicate" Duplicate
let pScratch : OperationParser = wordReturn "scratch" Scratch
let pLessThan : OperationParser = wordReturn "less than" LessThan
let pCall : OperationParser =
    pWord "call" >>. spaces >>. anyChar >>= fun name -> preturn (Call (name.ToString())) .>> spaces .>> pWord "now"
let pMaybe : OperationParser = pWord "maybe" >>. spaces >>. pCall >>= fun op -> preturn (Maybe op)
let pOperation : OperationParser =
    ignoreBabble >>. choice [pPush; pPull; pAdd; pSubtract; pMultiply; pDuplicate; pScratch; pCall; pLessThan; pMaybe]

type DefinitionParser = Parser<Definition,unit>
let pDefinition : DefinitionParser =
    pWord "define" >>. spaces >>. anyChar >>= fun name ->
        (spaces >>. pWord "as" >>. many1 pOperation >>= fun operations ->
            preturn (name.ToString(), operations))

type CommandParser = Parser<Command,unit>
let pUndo : CommandParser = wordReturn "undo" Undo
let pNop : CommandParser = wordReturn "uhm" Nop

let pRoot : CommandParser = choice  [
    pNop;
    pUndo;
    pOperation >>= fun x -> preturn (Op x);
    pDefinition >>= fun x -> preturn (Def x);
    ]

let runParser str =
    match run pRoot str with
    | Success(result, _, _)   -> Some result
    | Failure(errorMsg, _, _) -> 
        printfn "Parsing failure: %s" errorMsg
        None
