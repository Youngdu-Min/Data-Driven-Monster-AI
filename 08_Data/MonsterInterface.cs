public class MonsterInterface
{
    public interface ISetMultiplier
    {
        void SetMultiplier(float multiplier);
        void ResetMultiplier();
    }

    public interface IGetMediator
    {
        void SetMediator(MonsterMediator mediator);
    }
}
