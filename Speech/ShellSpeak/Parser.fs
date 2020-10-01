module Parser

open Language
open ParserLib

// Parse a string if it ends with a word boundary
let wordReturn word result = pWord word >>. pReturn result

let ignoreBabble =
    pOption <| (pOption pSpace)
    >>. pMany ((pOption pSpace) >>. (pWord "uhm"))

type CommandParser = Parser<Command>
let pGitStatus: CommandParser = wordReturn "git status" GitStatus

let pRoot: CommandParser = pChoice [ pGitStatus ]

let runParser str =
    match run pRoot str with
    | Success (result, _) -> Some result
    | Failure (msg, _) ->
        printfn "Parsing failure: %s" msg
        None
