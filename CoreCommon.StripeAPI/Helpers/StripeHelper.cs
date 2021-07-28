using CoreCommon.StripeAPI.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCommon.StripeAPI.Helpers
{
    public class StripeHelper
    {
        public string ApiKey { get; set; }

        public StripeConfig StripeConfig { get; set; }

        public string Token { get; set; }

        public StripeHelper(StripeConfig stripeConfig = null)
        {
            stripeConfig = new StripeConfig
            {
                SecretKey = "",
                PublishableKey = ""
            };
            StripeConfig = stripeConfig;
            StripeConfiguration.ApiKey = StripeConfig.SecretKey;
        }

        public RequestOptions GetOptions()
        {
            var requestOptions = new RequestOptions();
            requestOptions.ApiKey = "SECRET API KEY";
            requestOptions.IdempotencyKey = "SOME STRING";
            requestOptions.StripeAccount = "CONNECTED ACCOUNT ID";
            return requestOptions;
        }

        public void GetClient()
        {
            var stripeClient = new StripeClient(
                ApiKey
            //apiBase: "http://localhost:12111",
            //filesBase: "http://localhost:12111",
            //httpClient: new SystemNetHttpClient(maxNetworkRetries: 2)
            );
            StripeConfiguration.StripeClient = stripeClient;
        }

        public async Task<StripeList<Customer>> GetCustomers(string startingAfter = null)
        {
            var customerService = new CustomerService();
            var customers = await customerService.ListAsync(new CustomerListOptions
            {
                Limit = 100,
                StartingAfter = startingAfter
            });
            return customers;
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            var customerService = new CustomerService();
            var response = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Address = customer.Address != null ? new AddressOptions
                {
                    City = customer.Address.City,
                    Country = customer.Address.Country,
                    Line1 = customer.Address.Line1,
                    Line2 = customer.Address.Line2,
                    PostalCode = customer.Address.PostalCode,
                    State = customer.Address.State,
                } : null,
                Balance = customer.Balance,
                Description = customer.Description,
                Email = customer.Email,
                InvoicePrefix = customer.InvoicePrefix,
                Metadata = customer.Metadata,
                Name = customer.Name,
                Phone = customer.Phone,
                Validate = !customer.Deleted,
            });
            return response;
        }
        
        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            var customerService = new CustomerService();
            var response = await customerService.UpdateAsync(customer.Id, new CustomerUpdateOptions
            {
                Address = customer.Address != null ? new AddressOptions
                {
                    City = customer.Address.City,
                    Country = customer.Address.Country,
                    Line1 = customer.Address.Line1,
                    Line2 = customer.Address.Line2,
                    PostalCode = customer.Address.PostalCode,
                    State = customer.Address.State,
                } : null,
                Balance = customer.Balance,
                Description = customer.Description,
                Email = customer.Email,
                InvoicePrefix = customer.InvoicePrefix,
                Metadata = customer.Metadata,
                Name = customer.Name,
                Phone = customer.Phone,
                Validate = !customer.Deleted,
            });
            return response;
        }

        public async Task<StripeList<Product>> GetProducts(string startingAfter = null)
        {
            var productService = new ProductService();
            var products = await productService.ListAsync(new ProductListOptions
            {
                Limit = 100,
                StartingAfter = startingAfter
            });
            return products;
        }
        
        public async Task<Product> UpdateProductMetaData(string productId, Dictionary<string, string> metadata)
        {
            var productService = new ProductService();
            var products = await productService.UpdateAsync(productId, new ProductUpdateOptions
            {
                Metadata = metadata
            });
            return products;
        }

        public async Task<StripeList<Price>> GetPrices(string productId = null, string startingAfter = null)
        {
            var service = new PriceService();
            var items = await service.ListAsync(new PriceListOptions
            {
                Product = productId,
                Limit = 100,
                StartingAfter = startingAfter
            });
            return items;
        }

        public async Task<Price> UpdatePriceMetaData(string priceId, Dictionary<string, string> metadata)
        {
            var service = new PriceService();
            var response = await service.UpdateAsync(priceId, new PriceUpdateOptions
            {
                Metadata = metadata
            });
            return response;
        }

        public async Task<StripeList<Subscription>> GetSubscriptions(string startingAfter = null)
        {
            var service = new SubscriptionService();
            var items = await service.ListAsync(new SubscriptionListOptions
            {
                Limit = 100,
                StartingAfter = startingAfter
            });
            return items;
        }

        public async Task<Subscription> UpdateSubscriptionMetaData(string subscriptionId, Dictionary<string, string> metadata)
        {
            var service = new SubscriptionService();
            var response = await service.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
            {
                Metadata = metadata
            });
            return response;
        }

        public async Task<SubscriptionItem> UpdateSubscriptionItemMetaData(string subscriptionItemId, Dictionary<string, string> metadata)
        {
            var service = new SubscriptionItemService();
            var response = await service.UpdateAsync(subscriptionItemId, new SubscriptionItemUpdateOptions
            {
                Metadata = metadata
            });
            return response;
        }

        public async Task<StripeList<CustomerBalanceTransaction>> GetTransactions(string customerId, string startingAfter = null)
        {
            var service = new CustomerBalanceTransactionService();
            var items = await service.ListAsync(customerId, new CustomerBalanceTransactionListOptions
            {
                Limit = 100,
                StartingAfter = startingAfter
            });
            return items;
        }
        
        public async Task<StripeList<PaymentIntent>> GetPayments(string customerId, string startingAfter = null)
        {
            var service = new PaymentIntentService();
            var items = await service.ListAsync(new PaymentIntentListOptions
            {
                Customer = customerId,
                Limit = 100,
                StartingAfter = startingAfter
            });
            return items;
        }
        
        public async Task<StripeList<Charge>> GetCharges(string customerId, string startingAfter = null)
        {
            var service = new ChargeService();
            var items = await service.ListAsync(new ChargeListOptions
            {
                Customer = customerId,
                Limit = 100,
                StartingAfter = startingAfter
            });
            return items;
        }

        public async Task<Subscription> Subscribe(string customerId, string productId)
        {
            var service = new SubscriptionService();
            var prices = await GetPrices(productId);
            var item = await service.CreateAsync(new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = prices.Where(x => x.Type == "recurring").Select(x => new SubscriptionItemOptions
                {
                    Price = x.Id
                }).ToList(),
            });
            return item;
        }

        public async Task<Subscription> Subscribe(string customerId, string paymentMethod, List<SubscribeItemOption> items)
        {
            var service = new SubscriptionService();
            var item = await service.CreateAsync(new SubscriptionCreateOptions
            {
                Customer = customerId,
                DefaultPaymentMethod = paymentMethod,
                Items = items.Select(x => new SubscriptionItemOptions
                {
                    Price = x.PriceId,
                    Quantity = x.Quantity
                }).ToList(),
            });
            return item;
        }

        public async Task<Subscription> SubscriptionCancel(string subscriptionId)
        {
            var service = new SubscriptionService();
            var item = await service.CancelAsync(subscriptionId, new SubscriptionCancelOptions
            {

            });
            return item;
        }

        public async Task<StripeList<Plan>> GetPlans(string startingAfter = null)
        {
            var service = new PlanService();
            var items = await service.ListAsync(new PlanListOptions
            {
                Limit = 100,
                StartingAfter = startingAfter
            });
            return items;
        }

        public async Task<SetupIntent> CreateSetupIntent(string customerId)
        {
            var setupIntentOptions = new SetupIntentCreateOptions
            {
                Customer = customerId,
            };
            var setupIntentService = new SetupIntentService();
            var response = await setupIntentService.CreateAsync(setupIntentOptions);
            return response;
        }

        public async Task<PaymentMethod> CreatePaymentMethod(string customerId, string type, string startingAfter = null)
        {
            var service = new PaymentMethodService();
            var response = await service.CreateAsync(new PaymentMethodCreateOptions
            {
                Customer = customerId,
                Type = type,
                Card = new PaymentMethodCardOptions
                {

                }
            });
            return response;
        }

        public async Task<PaymentMethod> RemovePaymentMethod(string customerId, string paymentMethodId)
        {
            var service = new PaymentMethodService();
            var paymentMethod = await service.GetAsync(paymentMethodId);
            if (paymentMethod.CustomerId != customerId)
            {
                return null;
            }
            var response = await service.DetachAsync(paymentMethodId, new PaymentMethodDetachOptions
            {

            });
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="type">
        /// afterpay_clearpay, alipay, au_becs_debit,
        //     bacs_debit, bancontact, card, card_present, eps, fpx, giropay, grabpay, ideal,
        //     interac_present, oxxo, p24, sepa_debit, or sofort.
        //  </param>
        /// <param name="startingAfter"></param>
        /// <returns></returns>
        public async Task<StripeList<PaymentMethod>> GetPaymentMethods(string customerId, string type, string startingAfter = null)
        {
            var service = new PaymentMethodService();
            var items = await service.ListAsync(new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = type,
                Limit = 100,
                StartingAfter = startingAfter
            });
            return items;
        }

        public async Task<Charge> Charge(string customerId, long amount, string currency, string description)
        {
            var charges = new ChargeService();
            var charge = await charges.CreateAsync(new ChargeCreateOptions
            {
                Amount = amount,
                Currency = currency,
                Customer = customerId,
                Description = description
            });
            return charge;
        }

        // webhook
        public void ChargeChange(Stream stream)
        {
            var json = new StreamReader(stream).ReadToEnd();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, "Request.Headers[Stripe-Signature]", "WebhookSecret", throwOnApiVersionMismatch: true);
                Charge charge = (Charge)stripeEvent.Data.Object;
                switch (charge.Status)
                {
                    case "succeeded":
                        //This is an example of what to do after a charge is successful
                        charge.Metadata.TryGetValue("Product", out string Product);
                        charge.Metadata.TryGetValue("Quantity", out string Quantity);


                        break;
                    case "failed":
                        //Code to execute on a failed charge
                        break;
                }
            }
            catch (Exception e)
            {

            }

        }
    }
}
