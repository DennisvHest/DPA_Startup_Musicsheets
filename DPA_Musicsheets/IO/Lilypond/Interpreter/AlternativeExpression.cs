namespace DPA_Musicsheets.IO.Lilypond.Interpreter
{
    class AlternativeExpression : LilypondSection
    {
        public override void Interpret(LilypondContext context)
        {
            foreach (Expression expression in ChildExpressions)
            {
                expression.Interpret(context);
            }
        }
    }
}
