module ParserLib

type Input = char list
type Message = string * bool

type ParseResult<'T> =
    | Success of 'T * Input
    | Failure of Message

type Parser<'T> = Input -> ParseResult<'T>

let run<'T> (parser: Parser<'T>) (input: string) = Seq.toList input |> parser

let (>>=) (p1: Parser<'a>) (fp2: 'a -> Parser<'b>) (input: Input) =
    match p1 input with
    | Success (result, rest) -> fp2 result rest
    | Failure msg -> Failure msg

let pReturn result = fun input -> Success(result, input)
let pBacktrack msg = fun _ -> Failure(msg, true)
let pFatal msg = fun _ -> Failure(msg, false)

let pCommit p input =
    match p input with
    | Failure (innerMsg, backtrack) as failure -> if backtrack then Failure(innerMsg, false) else failure
    | success -> success

let pIgnore p =
    p >>= fun _ -> fun rest -> Success((), rest)

let (.>>) p1 p2 =
    p1 >>= fun result -> p2 >>= fun _ -> pReturn result

let (>>.) p1 p2 =
    p1 >>= fun _ -> p2 >>= fun result -> pReturn result

let (<?>) p msg input =
    match p input with
    | Failure (innerMsg, backtrack) -> if backtrack then Failure(msg, backtrack) else Failure(innerMsg, backtrack)
    | success -> success

let (<|>) p1 p2 input =
    match p1 input with
    | Failure (msg, backtrack) as failure ->
        if not backtrack then
            failure
        else
            match p2 input with
            | Failure (msg2, backtrack) as failure2 ->
                if backtrack
                then Failure(sprintf "%s or %s" msg msg2, backtrack)
                else failure2
            | success -> success
    | success -> success

let pOption p =
    (p >>= fun result -> pReturn <| Some result) <|> pReturn None

let lookAhead (p: Parser<'a>) (input: Input) =
    (p >>= fun _ -> fun _ -> Success((), input)) input

let pChar (c: char): Parser<char> =
    fun input ->
        match input with
        | x :: xs when x = c -> Success(x, xs)
        | _ -> Failure <| (sprintf "expected %A" c, true)

let pAnyChar: Parser<char> =
    fun input ->
        match input with
        | x :: xs -> Success(x, xs)
        | _ -> Failure <| (sprintf "expected character", true)

let pDigit: Parser<char> =
    fun input ->
        let inline charToInt c = int c - int '0'

        match input with
        | x :: xs when (charToInt x >= 0 && charToInt x < 10) -> Success(x, xs)
        | _ -> Failure <| (sprintf "expected digit", true)

let rec pChars chars =
    match chars with
    | [] -> pReturn []
    | x :: xs -> pChar x >>= fun result -> pChars xs >>= fun recResult -> pReturn (result :: recResult)

let rec pChoice ps =
    match ps with
    | [] -> pFatal "No choice given."
    | p :: [] -> p
    | p :: ps -> p <|> pChoice ps

let rec pMany (p: Parser<'a>) (input: Input): ParseResult<'a list> =
    match p input with
    | Success (result, rest) -> (pMany p >>= fun recResult -> pReturn (result :: recResult)) rest
    | Failure _ -> Success([], input)

let pMany1 p =
    p >>= fun result -> (pMany p) >>= fun recResult -> pReturn (result :: recResult)

let pSpace = pChar ' '

let pString (str: string) =
    pChars [ for c in str -> c ] >>= fun resultChars -> pReturn (System.String.Concat(resultChars))

let pEndOfInput input =
    match input with
    | [] -> Success((), input)
    | _ -> Failure("expected end of input", true)

let pWordBoundary = (lookAhead pSpace <|> pEndOfInput)

let pWord str =
    pString str .>> pWordBoundary <?> (sprintf "expected word '%s'" str)

let pInt =
    pMany1 pDigit
    >>= fun resultDigits -> pReturn (int <| System.String.Concat(resultDigits)) .>> pWordBoundary
