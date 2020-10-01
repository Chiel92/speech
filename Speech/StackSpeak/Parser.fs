module Parser

open Language
open ParserLib

// Parse a string if it ends with a word boundary
let wordReturn word result = pWord word >>. pReturn result

let ignoreBabble =
    pOption <| (pOption pSpace)
    >>. pMany ((pOption pSpace) >>. (pWord "uhm"))

type OperationParser = Parser<Operation>

let pPush: OperationParser =
    pWord "push"
    >>. pCommit (pSpace >>. pInt >>= fun x -> pReturn (Push x))

let pPull: OperationParser =
    pWord "pull"
    >>. pCommit (pSpace >>. pInt >>= fun x -> pReturn (Pull x))

let pAdd: OperationParser = wordReturn "add together" Add
let pSubtract: OperationParser = wordReturn "subtract" Subtract
let pMultiply: OperationParser = wordReturn "multiply" Multiply
let pDuplicate: OperationParser = wordReturn "duplicate" Duplicate
let pScratch: OperationParser = wordReturn "scratch" Scratch
let pLessThan: OperationParser = wordReturn "less than" LessThan

let pCall: OperationParser =
    pWord "call"
    >>. pCommit
            (pSpace >>. pAnyChar
             >>= fun name ->
                     pReturn (Call(name.ToString()))
                     .>> pSpace
                     .>> pWord "now")

let pMaybe: OperationParser =
    pWord "maybe" >>. pSpace >>. pCall
    >>= fun op -> pReturn (Maybe op)

let pOperation: OperationParser =
    ignoreBabble
    >>. (pChoice [ pPush
                   pPull
                   pAdd
                   pSubtract
                   pMultiply
                   pDuplicate
                   pScratch
                   pCall
                   pLessThan
                   pMaybe ])
    <?> "expected operation"

type DefinitionParser = Parser<Definition>

let pDefinition: DefinitionParser =
    (pWord "define"
     >>. pCommit
             (pSpace >>. pAnyChar
              >>= fun name ->
                      (pSpace >>. pWord "as" >>. pMany1 pOperation
                       >>= fun operations -> pReturn (name.ToString(), operations)))
     <?> "expected definition")

type CommandParser = Parser<Command>
let pUndo: CommandParser = wordReturn "undo" Undo
let pNop: CommandParser = wordReturn "uhm" Nop

let pRoot: CommandParser =
    pChoice [ pNop
              pUndo
              pOperation >>= fun x -> pReturn (Op x)
              pDefinition >>= fun x -> pReturn (Def x) ]

let runParser str =
    match run pRoot str with
    | Success (result, _) -> Some result
    | Failure (msg, _) ->
        printfn "Parsing failure: %s" msg
        None
