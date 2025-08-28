namespace Tests.Core
{
    public abstract class TestBase
    {


        protected const int QuoteId = 1;


        [OneTimeSetUp]
        public void SetUp()
        {
            
        }


        [OneTimeTearDown]
        public void BaseTearDown()
        {

        }
    }
}
