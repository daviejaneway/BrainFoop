using System;
using System.Collections.Generic;

namespace BrainFoop
{
    enum TokenType {
        INC,
        DEC,
        FWD,
        BCK,
        LBR,
        RBR,
        PUT,
        GET
    }

    struct Token {
        public TokenType Type;
        public string Value;

        public Token(TokenType type, string value) {
            this.Type = type;
            this.Value = value;
        }

        public override bool Equals(object obj) {
            return ((Token)obj).Type == this.Type;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }

    class Lexer {

        public List<Token> Lex(string source) {
            List<Token> tokens = new List<Token>();

            foreach (char ch in source.ToCharArray()) {
                switch (ch) {
                    case '+': tokens.Add(new Token(TokenType.INC, "+")); break;
                    case '-': tokens.Add(new Token(TokenType.DEC, "-")); break;
                    case '>': tokens.Add(new Token(TokenType.FWD, ">")); break;
                    case '<': tokens.Add(new Token(TokenType.BCK, "<")); break;
                    case '.': tokens.Add(new Token(TokenType.PUT, ".")); break;
                    case ',': tokens.Add(new Token(TokenType.GET, ",")); break;
                    case '[': tokens.Add(new Token(TokenType.LBR, "[")); break;
                    case ']': tokens.Add(new Token(TokenType.RBR, "]")); break;
                }
            }

            return tokens;
        }
    }

    class Program {
        private int[] Tape = new int[1024];
        private int CellPtr = 0;

        void Inc() {
            Tape[CellPtr] += 1;
        }

        void Dec() {
            if (Tape[CellPtr] > 0) {
                Tape[CellPtr] -= 1;
            }
        }

        void Fwd() {
            // Wrap around to simulate infinite tape
            if (CellPtr >= Tape.Length) {
                CellPtr = 0;
            } else {
                CellPtr += 1;
            }
        }

        void Bck() {
            // Wrap around to simulate infinite tape
            if (CellPtr <= 0) {
                CellPtr = Tape.Length - 1;
            } else {
                CellPtr -= 1;
            }
        }

        void Put() {
            Console.WriteLine((char)Tape[CellPtr]);
        }

        void Get() {
            int ch = Console.Read();
            Tape[CellPtr] = ch;
        }

        public void Run(List<Token> tokens) {
            Token token;
            for (int i = 0; i < tokens.Count; i++) {
                token = tokens[i];
                switch (token.Type) {
                    case TokenType.INC: Inc(); break;
                    case TokenType.DEC: Dec(); break;
                    case TokenType.FWD: Fwd(); break;
                    case TokenType.BCK: Bck(); break;
                    case TokenType.PUT: Put(); break;
                    case TokenType.GET: Get(); break;
                    
                    case TokenType.LBR:
                        int idx = tokens.LastIndexOf(new Token(TokenType.RBR, "]"));

                        if (i >= idx) {
                            // WEIRDNESS
                            Console.WriteLine("FATAL ERROR: Loop is closed before it is opened!");
                            System.Environment.Exit(1);
                            break;
                        }

                        if (idx > -1) {
                            int diff = idx - i;
                            List<Token> sub = tokens.GetRange(i + 1, diff - 1);

                            while (Tape[CellPtr] > 0) {
                                Run(sub);
                            }

                            // Remove the loop we've just run
                            tokens.RemoveRange(i, diff);
                        } else {
                            // This might be illegal
                            List<Token> sub = tokens.GetRange(i, tokens.Count - 1);
                            while (true) {
                                Run(sub);
                            }
                        }

                        break;
                }
            }
        }
    }

    class Repl {
        public void Run() {
            Lexer lexer = new Lexer();

            string line;
            List<Token> tokens;
            Program program;
            Console.Write(">> ");
            while((line = Console.ReadLine()) != null) {
                if (line == "q") {
                    break;
                }

                tokens = lexer.Lex(line);
                program = new Program();

                program.Run(tokens);
                Console.Write(">> ");
            }
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            Repl repl = new Repl();
            repl.Run();
        }
    }
}
