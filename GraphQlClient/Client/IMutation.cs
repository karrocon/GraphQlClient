namespace GraphQlClient
{
    public interface IMutation<TResult>
    {
        GraphQlRequestMessage ToGraphQlRequestMessage();
    }
}