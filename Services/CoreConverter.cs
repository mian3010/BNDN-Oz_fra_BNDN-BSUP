using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.FSharp.Core;

namespace RentIt.Services
{
    public class CoreConverter
    {
        private Helper h;

        public CoreConverter(Helper helper) {

            h = helper;
        }

        public TokenData Convert(Tuple<string, DateTime> t)
        {
            if (t == null) return null;

            string token = t.Item1;
            string expires = JsonUtility.dateTimeToString(t.Item2);

            return new TokenData() { token = token, expires = expires };
        }

        public AccountData Convert(AccountTypes.Account a, DateTime? authenticated)
        {
            if (a == null) return null;

            return new AccountData()
            {
                user = a.user,
                email = a.email,
                password = null,
                type = a.accType,
                banned = a.banned,
                name = h.OrNull(a.info.name),
                address = Convert(a.info.address),
                birth = h.OrNull(a.info.birth, JsonUtility.dateToString),
                about = h.OrNull(a.info.about),
                credits = h.OrNulled(a.info.credits, i => (uint)i),
                created = JsonUtility.dateTimeToString(a.created),
                authenticated = h.OrNull(authenticated, JsonUtility.dateTimeToString)
            };
        }

        public AccountTypes.Account Convert(AccountData a)
        {
            if (a == null) return null;

            var dummy = Account.make("", "", "", "", ConvertExtra(null));

            return Merge(dummy, a);
        }

        public AccountTypes.Account Merge(AccountTypes.Account src, AccountData update)
        {
            if (update == null) return src;
            if (src == null) return Convert(update);

            return new AccountTypes.Account(

                update.user == null ? src.user : update.user,
                update.email == null ? src.email : update.email,
                update.password == null ? src.password : Account.Password.create(update.password),
                update.created == null ? src.created : JsonUtility.stringToDateTime(update.created),
                update.banned == null ? src.banned : (bool) update.banned,
                Merge(src.info, update),
                update.type == null ? src.accType : update.type,
                src.version
            );
        }

        public AccountTypes.ExtraAccInfo ConvertExtra(AccountData a)
        {
            if (a == null) return new AccountTypes.ExtraAccInfo(
                
                                FSharpOption<string>.None,
                                Convert((AddressData) null),
                                FSharpOption<DateTime>.None,
                                FSharpOption<string>.None,
                                FSharpOption<int>.None
                            );

            return new AccountTypes.ExtraAccInfo(

                new FSharpOption<string>(a.name),
                Convert(a.address),
                FSharpOption<DateTime>.Some(JsonUtility.stringToDate(a.birth)),
                new FSharpOption<string>(a.about),
                a.credits == null ? FSharpOption<int>.None : FSharpOption<int>.Some((int) a.credits)
            );
        }

        public AccountTypes.ExtraAccInfo Merge(AccountTypes.ExtraAccInfo src, AccountData update)
        {
            if (update == null) return src;
            if (src == null) return ConvertExtra(update);

            return new AccountTypes.ExtraAccInfo(

                update.name == null ? src.name : new FSharpOption<string>(update.name),
                Merge(src.address, update.address),
                update.birth == null ? src.birth : new FSharpOption<DateTime>(JsonUtility.stringToDate(update.created)),
                update.about == null ? src.about : new FSharpOption<string>(update.about),
                update.credits == null ? src.credits : FSharpOption<int>.Some((int) update.credits)
            );
        }

        public AddressData Convert(AccountTypes.Address a)
        {
            if (a == null) return null;

            return new AddressData()
            {
                address = h.OrNull(a.address),
                postal = h.OrNulled(a.postal, i => (uint)i),
                country = h.OrNull(a.country)
            };
        }

        public AccountTypes.Address Convert(AddressData a)
        {
            if (a == null) return new AccountTypes.Address(

                                FSharpOption<string>.None,
                                FSharpOption<int>.None,
                                FSharpOption<string>.None
                            );

            return new AccountTypes.Address(

                new FSharpOption<string>(a.address),
                a.postal == null ? FSharpOption<int>.None : FSharpOption<int>.Some((int) a.postal),
                new FSharpOption<string>(a.country)
            );
        }

        public AccountTypes.Address Merge(AccountTypes.Address src, AddressData update)
        {
            if (update == null) return null;
            if (src == null) return Convert(update);

            return new AccountTypes.Address(
                
                update.address == null ? src.address : new FSharpOption<string>(update.address),
                update.postal == null ? src.postal : FSharpOption<int>.Some((int) update.postal),
                update.country == null ? src.country : new FSharpOption<string>(update.country)
            );
        }

//        public ProductData Convert()
//        {
//            return new ProductData {
            
                
//            };

////[DataContract]
////public class ProductData
////{
////    [DataMember]
////    public string title { get; set; }
////    [DataMember]
////    public string description { get; set; }
////    [DataMember]
////    public string type { get; set; }
////    [DataMember]
////    public PriceData price { get; set; }
////    [DataMember]
////    public RatingData rating { get; set; }
////    [DataMember]
////    public string owner { get; set; }
////    [DataMember]
////    public MetaData[] meta { get; set; }
////    [DataMember]
////    public bool? published { get; set; }
////}
//        }

//        public PriceData Convert()
//        {
//            return new PriceData {
            
                
//            };

////[DataContract]
////public class PriceData
////{
////    [DataMember]
////    public uint buy { get; set; }
////    [DataMember]
////    public uint? rent { get; set; }
////}
//        }

//        public RatingData Convert()
//        {
//            return new RatingData {
            
                
//            };
////[DataContract]
////public class RatingData
////{
////    [DataMember]
////    public int score { get; set; }
////    [DataMember]
////    public uint count { get; set; }
////}
//        }

//        public MetaData Convert()
//        {
//            return new MetaData {
            
                
//            };
////[DataContract]
////public class MetaData
////{
////    [DataMember]
////    public string name { get; set; }
////    [DataMember]
////    public string value { get; set; }
////}
//        }

//        public PurchaseData Convert()
//        {
//            return new PurchaseData {
            
                
//            };

////[DataContract]
////public class PurchaseData
////{
////    [DataMember]
////    public string purchased { get; set; }
////    [DataMember]
////    public uint paid { get; set; }
////    [DataMember]
////    public string type { get; set; }
////    [DataMember]
////    public string expires { get; set; }
////    [DataMember]
////    public string title { get; set; }
////    [DataMember]
////    public uint product { get; set; }
////}
//        }

        public IdData Convert(uint id)
        {
            return new IdData {
            
                id = id
            };
        }
    }
}