namespace EasyParse.Parsing.Rules
{
    public interface IEmptyRule
    {
        Rule Match(params Symbol[] symbols);
    }
}
