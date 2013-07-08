using System;
using Eto.Parse.Testers;
using System.Collections.Generic;

namespace Eto.Parse.Parsers
{
	public class CharParser : Parser, IInverseParser
	{
		public bool Inverse { get; set; }

		public ICharTester Tester { get; set; }

		protected CharParser(CharParser other)
			: base(other)
		{
			Tester = other.Tester;
		}

		public override string DescriptiveName
		{
			get
			{
				var tester = Tester != null ? Tester.GetType().Name : null;
				return string.Format("{0}, Tester: {1}", base.DescriptiveName, tester);
			}
		}

		public CharParser()
		{
		}

		public CharParser(ICharTester tester)
		{
			this.Tester = tester;
		}

		protected override ParseMatch InnerParse(ParseArgs args)
		{
			var scanner = args.Scanner;
			char ch;
			int pos;
			if (scanner.ReadChar(out ch, out pos))
			{
				bool matched = Tester.Test(ch, args.Grammar.CaseSensitive);
				if (matched != Inverse)
					return new ParseMatch(pos, 1);
			}
			scanner.Position = pos;
			return args.NoMatch;
		}

		public override IEnumerable<NamedParser> Find(string parserId)
		{
			yield break;
		}

		public override Parser Clone()
		{
			return new CharParser(this);
		}

		public static CharParser operator +(CharParser parser, CharParser include)
		{
			return new CharParser(new IncludeTester(parser.Tester, parser.Inverse, include.Tester, include.Inverse)) { Reusable = true };
		}

		public static CharParser operator +(CharParser parser, char[] chars)
		{
			return new CharParser(new IncludeTester(parser.Tester, parser.Inverse, new CharSetTester(chars), false)) { Reusable = true };
		}

		public static CharParser operator +(CharParser parser, char ch)
		{
			return new CharParser(new IncludeTester(parser.Tester, parser.Inverse, new CharSetTester(ch), false)) { Reusable = true };
		}

		public static CharParser operator -(CharParser include, CharParser exclude)
		{
			return new CharParser(new ExcludeTester(include.Tester, include.Inverse, exclude.Tester, exclude.Inverse)) { Reusable = true };
		}

		public static CharParser operator -(CharParser include, char[] chars)
		{
			return new CharParser(new ExcludeTester(include.Tester, include.Inverse, new CharSetTester(chars), false)) { Reusable = true };
		}

		public static CharParser operator -(CharParser include, char ch)
		{
			return new CharParser(new ExcludeTester(include.Tester, include.Inverse, new CharSetTester(ch), false)) { Reusable = true };
		}
	}
}
