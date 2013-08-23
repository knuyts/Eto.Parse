using System;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Parse
{
	public class Match
	{
		MatchCollection matches;
		static Match empty;
		ParseMatch parseMatch;

		public static Match EmptyMatch
		{
			get { return empty ?? (empty = new Match(null, null, null, new ParseMatch(-1, -1), new MatchCollection())); }
		}

		public MatchCollection Matches
		{
			get { return matches ?? (matches = new MatchCollection()); }
		}

		public Scanner Scanner { get; private set; }

		public object Value { get { return Success ? Parser.GetValue(this) : null; } }

		public string StringValue { get { return Convert.ToString(Value); } }

		public string Text { get { return Success ? Scanner.SubString(Index, Length) : null; } }

		public string Name { get; private set; }

		public Parser Parser { get; private set; }

		public object Tag { get; set; }

		public int Index { get { return parseMatch.Index; } }

		public int Length { get { return parseMatch.Length; } }

		public bool Success { get { return parseMatch.Success; } }

		public bool Empty { get { return parseMatch.Empty; } }


		internal Match(string name, Parser parser, Scanner scanner, ParseMatch parseMatch, MatchCollection matches)
		{
			this.Name = name;
			this.Parser = parser;
			this.Scanner = scanner;
			this.parseMatch = parseMatch;
			this.matches = matches;
		}

		public IEnumerable<Match> Find(string id, bool deep = false)
		{
			if (matches != null)
				return matches.Find(id, deep);
			else
				return Enumerable.Empty<Match>();
		}

		public Match this [string id, bool deep = false]
		{
			get
			{
				if (matches != null)
					return matches[id, deep];
				else
					return Match.EmptyMatch;
			}
		}

		internal void TriggerPreMatch()
		{
			if (matches != null)
				matches.ForEach(r => r.TriggerPreMatch());
			Parser.TriggerPreMatch(this);
		}

		internal void TriggerMatch()
		{
			if (matches != null)
				matches.ForEach(r => r.TriggerMatch());
			Parser.TriggerMatch(this);
		}

		public override string ToString()
		{
			return Text ?? string.Empty;
		}

		public static bool operator true(Match match)
		{
			return match.Success;
		}

		public static bool operator false(Match match)
		{
			return !match.Success;
		}
	}

	public class MatchCollection : List<Match>
	{
		public MatchCollection()
			: base(4)
		{
		}

		public IEnumerable<Match> Find(string id, bool deep = false)
		{
			var matches = this.Where(r => r.Name == id);
			if (deep && !matches.Any())
				return matches.Concat(this.SelectMany(r => r.Find(id, deep)));
			else
				return matches;
		}

		public Match this [string id, bool deep = false]
		{
			get { return Find(id, deep).FirstOrDefault() ?? Match.EmptyMatch; }
		}
	}
}
