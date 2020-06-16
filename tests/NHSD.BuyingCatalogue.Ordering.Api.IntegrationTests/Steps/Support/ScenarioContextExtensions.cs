﻿using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support
{
    internal static class ScenarioContextExtensions
    {
        public static TValue Get<TValue>(this ScenarioContext context, string key, TValue defaultValue) =>
            context.TryGetValue(key, out TValue value) ? value : defaultValue;

        public static int? GetContactIdByEmail(this ScenarioContext context, string email)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (email == null)
                return null;

            var contactDictionary = context.Get<IDictionary<string, int>>(ScenarioContextKeys.ContactMapDictionary, new Dictionary<string, int>());
            if (contactDictionary.TryGetValue(email, out var valueId))
                return valueId;

            return null;
        }

        public static int? GetAddressIdByPostcode(this ScenarioContext context, string postcode)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (postcode == null)
                return null;

            var contactDictionary =
                context.Get<IDictionary<string, int>>(ScenarioContextKeys.AddressMapDictionary, new Dictionary<string, int>());
            
            if (contactDictionary.TryGetValue(postcode, out var valueId))
                return valueId;

            return null;
        }

        public static int? GetOrderIdByDescription(this ScenarioContext context, string description)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (description == null)
                return null;

            var orderDictionary =
                context.Get<IDictionary<string, int>>(ScenarioContextKeys.OrderMapDictionary, new Dictionary<string, int>());

            if (orderDictionary.TryGetValue(description, out var valueId))
                return valueId;

            return null;
        }

        public static void SetOrderIdByDescription(this ScenarioContext context, string description, int orderId)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (description == null)
                return;

            var OrderMapDictionary =
                context.Get<IDictionary<string, int>>(ScenarioContextKeys.OrderMapDictionary, new Dictionary<string, int>());

            OrderMapDictionary[description] = orderId;

            if (!context.ContainsKey(ScenarioContextKeys.OrderMapDictionary))
            {
                context[ScenarioContextKeys.OrderMapDictionary] = OrderMapDictionary;
            }
        }
    }
}
