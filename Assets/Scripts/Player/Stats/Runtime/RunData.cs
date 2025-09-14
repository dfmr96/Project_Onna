namespace Player.Stats.Runtime
{
    public static class RunData
    {
        //public static RuntimeStats CurrentStats { get; private set; }
        public static RunCurrency CurrentCurrency { get; private set; }
        public static NewMutationController NewMutationController { get; private set; }

        public static void Initialize()
        {
            if (CurrentCurrency == null) CurrentCurrency = new RunCurrency();
            //if (NewMutationController == null) NewMutationController = new NewMutationController();
        }
        //public static void SetStats(RuntimeStats stats)
        //{
        //    CurrentStats = stats;
        //}
        public static void Clear() 
        { 
            //CurrentStats = null;
            CurrentCurrency = null;
        }
    }
}