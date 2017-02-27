using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.NUnit2;

namespace LightBDD.Example.AcceptanceTests.NUnit2.Features
{
    [FeatureDescription(
@"In order to get desired products
As a customer
I want to pay for products in basket")]
    [Label("Story-5")]
    public partial class Payment_feature
    {
        [Scenario]
        [Label("Ticket-10"), Label("Ticket-11")]
        public void Successful_payment()
        {
            Runner.RunScenarioAsync(
              _=> Given_customer_has_some_products_in_basket(),
              _=> Given_customer_has_enough_money_to_pay_for_products(),
              _=> When_customer_requests_to_pay(),
              _=> Then_payment_should_be_successful());
        }
    }
}