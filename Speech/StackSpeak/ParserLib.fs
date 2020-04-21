module ParserLib

type Input = char list
type Message = string
type ParseResult<'T> = Success of 'T * Input | Failure of Message
type Parser<'T> = Input -> ParseResult<'T>

let runParser<'T> (parser:Parser<'T>) (input:string) =
    Seq.toList input |> parser

let (<|>) p1 p2 input =
    match p1 input with
    | Failure error -> 
        match p2 input with
        | Failure error2 -> Failure <| sprintf "%s or %s" error error2
        | success -> success
    | success -> success

let (>>=) (p1:Parser<'a>) (fp2:'a -> Parser<'b>) (input:Input) =
    match p1 input with
    | Success (result, rest) -> fp2 result rest
    | Failure msg -> Failure msg

let pReturn result = fun rest -> Success (result, rest)
let pIgnore p = p >>= fun _ -> fun rest -> Success ((), rest)
let pOption p = (p >>= fun result -> pReturn <| Some result) <|> pReturn None

let (.>>) p1 p2 = p1 >>= fun result -> p2 >>= fun _ -> pReturn result
let (>>.) p1 p2 = p1 >>= fun _ -> p2 >>= fun result -> pReturn result

let lookAhead (p1:Parser<'a>) (input:Input) =
    p1 >>= fun result -> fun _ -> Success (result, input)

let pChar (c:char) : Parser<char> = fun input ->
    match input with
    | x::xs when x = c -> Success (x, xs)
    | _ -> Failure <| sprintf "Expected %A" c

let pDigit : Parser<char> = fun input ->
    let inline charToInt c = int c - int '0'
    match input with
    | x::xs when (charToInt x >= 0 && charToInt x < 10) -> Success (x, xs)
    | _ -> Failure <| sprintf "Expected digit"

let rec pChars chars = 
    match chars with
    | [] -> pReturn []
    | x::xs -> pChar x >>= fun result -> pChars xs >>= fun recResult -> pReturn (result::recResult)

let rec pMany (p:Parser<'a>) (input:Input) : ParseResult<'a list> = 
    match p input with
    | Success (result, rest) -> (pMany p >>= fun recResult -> pReturn (result :: recResult)) <| rest
    | Failure _ -> Success ([], input)

let pSpace = pChar ' '
let pString (str:string) = pChars [for c in str -> c] >>= fun result ->
    pReturn (System.String.Concat(result))

let pEndOfInput input =
    match input with
    | [] -> Success ((), input)
    | _ -> Failure "Expected end of input"

let pWord str = pString str .>> (pIgnore pSpace <|> pEndOfInput)
let pInt = pMany pDigit >>= fun digits ->
    pReturn (int <| System.String.Concat(digits)) .>> (pIgnore pSpace <|> pEndOfInput)

let pHello = pWord "Hello" >>. pInt
