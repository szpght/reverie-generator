using System.Collections.Generic;
using System.Linq;

namespace Reverie.CodeGeneration
{
    public enum RelationType
    {
        And,
        Or,
    }

    public class Relation : IPredicate
    {
        public IPredicate Left { get; }
        public IPredicate Right { get; }
        public bool Negated { get; set; }
        public bool JumpToElse { get; set; }
        public RelationType Type { get; private set; }
        public string Jump => null;

        private Relation LeftAsRelation => Left as Relation;
        private Relation RightAsRelation => Right as Relation;

        public Relation(IPredicate left, IPredicate right, RelationType type, bool negated = false)
        {
            Left = left;
            Right = right;
            Type = type;
            Negated = negated;
        }

        /* wstępny algorytm:
         * 1. przechodzenie drzewa post-order
         * 2. jeśli węzeł jest andem, to zmieniam go na nor i neguję dzieci
         *   2.1. Negując dziecko zmieniam nor na or i odwrotnie
         * 3. po przejściu całego drzewa mam tylko ory i nory
         * 4. przechodzę drzewo znowu i jeśli spotykam nora to neguję JumpToElse wszystkim wychodzącym z niego liściom
         */
        public void NormalizeToOr()
        {
            var relations = new List<Relation>();
            MakePostOrderListOfRelations(relations);
            foreach (var relation in relations)
            {
                relation.ConvertRelationType();
            }
            foreach (var relation in relations.Where(x => x.Type == RelationType.Or && x.Negated))
            {
                relation.NegateJumpInLeaves();
            }
        }

        private void MakePostOrderListOfRelations(List<Relation> relations)
        {
            LeftAsRelation?.MakePostOrderListOfRelations(relations);
            RightAsRelation?.MakePostOrderListOfRelations(relations);
            relations.Add(this);
        }

        private void ConvertRelationType()
        {
            if (Type == RelationType.And)
            {
                Type = RelationType.Or;
                Negated = !Negated;
                if (LeftAsRelation != null)
                {
                    LeftAsRelation.Negated = !LeftAsRelation.Negated;
                    RightAsRelation.Negated = !RightAsRelation.Negated;
                }
                else
                {
                    Left.Negated = !Left.Negated;
                    Right.Negated = !Right.Negated;
                }
            }
        }

        private void NegateJumpInLeaves()
        {
            LeftAsRelation?.NegateJumpInLeaves();
            RightAsRelation?.NegateJumpInLeaves();
            if (LeftAsRelation == null)
            {
                Left.JumpToElse = !Left.JumpToElse;
                Right.JumpToElse = !Right.JumpToElse;
            }
        }

        public Assembly Generate(Context ctx)
        {
            throw new System.NotImplementedException();
        }

        public void Generate(Assembly asm, Context ctx)
        {
        }
    }
}