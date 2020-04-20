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

let success result = fun rest -> Success (result, rest)

let (.>>) p1 p2 = p1 >>= fun result -> p2 >>= fun _ -> success result
let (>>.) p1 p2 = p1 >>= fun _ -> p2 >>= fun result -> success result

let pChar (c:char) : Parser<char> = fun s ->
    match s with
    | x::xs when x = c -> Success (x, xs)
    | _ -> Failure <| sprintf "Expected %A" c

let rec pChars chars = 
    match chars with
    | [] -> success []
    | x::xs -> pChar x >>= fun result -> pChars xs >>= fun recResult -> success (result::recResult)

let pString (str:string) = pChars [for c in str -> c] >>= fun result ->
    success (System.String.Concat(result))

let pHello = pString "Hello"
