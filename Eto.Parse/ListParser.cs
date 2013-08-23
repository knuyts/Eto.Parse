using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Parse
{
	public abstract class ListParser : Parser
	{
		public List<Parser> Items { get; private set; }

		protected ListParser(ListParser other, ParserCloneArgs chain)
			: base(other, chain)
		{
			Items = new List<Parser>(other.Items.Select(r => chain.Clone(r)));
		}

		public ListParser()
		{
			Items = new List<Parser>();
		}

		public ListParser(IEnumerable<Parser> sequence)
		{
			Items = sequence.ToList();
		}

		public override IEnumerable<Parser> Find(ParserFindArgs args)
		{
			var ret = base.Find(args);
			if (args.Push(this)) 
			{
				ret = ret.Concat(Items.Where(r => r != null).SelectMany(r => r.Find(args)).ToArray());
				args.Pop(this);
			}
			return ret;
		}

		public void InitializeItems(ParserInitializeArgs args)
		{
			foreach (var item in Items)
			{
				if (item != null)
					item.Initialize(args);
			}
		}

		public override bool Contains(ParserContainsArgs args)
		{
			if (base.Contains(args))
				return true;
			if (args.Push(this))
			{
				foreach (var item in Items)
				{
					if (item != null && item.Contains(args))
					{
						args.Pop(this);
						return true;
					}
				}
				args.Pop(this);
			}
			return false;
		}

		public override IEnumerable<Parser> Children(ParserChildrenArgs args)
		{
			if (args.Push(this))
			{
				var items = Items.Where(r => r != null);
				var childItems = items.SelectMany(r => r.Children(args)).ToArray();
				args.Pop(this);
				return items.Concat(childItems);
			}
			return Enumerable.Empty<Parser>();
		}

		public void Add(params Parser[] parsers)
		{
			Items.AddRange(parsers);
		}
	}
}

