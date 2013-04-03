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

        #region TokenData

        public TokenData Convert(Tuple<string, DateTime> t)
        {
            if (t == null) return null;

            string token = t.Item1;
            string expires = JsonUtility.dateTimeToString(t.Item2);

            return new TokenData() { token = token, expires = expires };
        }

        #endregion

        #region AccountData

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
                address = h.OrNull(a.info.address.address),
                postal = h.OrNulled(a.info.address.postal, i => (uint)i),
                country = h.OrNull(a.info.address.country),
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

            var dummy = Account.make("", "", "", "", ConvertToExtra(null));

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

        public AccountTypes.ExtraAccInfo ConvertToExtra(AccountData a)
        {
            if (a == null) return new AccountTypes.ExtraAccInfo(
                
                                FSharpOption<string>.None,
                                ConvertToAddress((AccountData) null),
                                FSharpOption<DateTime>.None,
                                FSharpOption<string>.None,
                                FSharpOption<int>.None
                            );

            return new AccountTypes.ExtraAccInfo(

                new FSharpOption<string>(a.name),
                ConvertToAddress((AccountData) a),
                FSharpOption<DateTime>.Some(JsonUtility.stringToDate(a.birth)),
                new FSharpOption<string>(a.about),
                a.credits == null ? FSharpOption<int>.None : FSharpOption<int>.Some((int) a.credits)
            );
        }

        public AccountTypes.ExtraAccInfo Merge(AccountTypes.ExtraAccInfo src, AccountData update)
        {
            if (update == null) return src;
            if (src == null) return ConvertToExtra(update);

            return new AccountTypes.ExtraAccInfo(

                update.name == null ? src.name : new FSharpOption<string>(update.name),
                Merge(src.address, update),
                update.birth == null ? src.birth : new FSharpOption<DateTime>(JsonUtility.stringToDate(update.created)),
                update.about == null ? src.about : new FSharpOption<string>(update.about),
                update.credits == null ? src.credits : FSharpOption<int>.Some((int) update.credits)
            );
        }

        public AccountTypes.Address ConvertToAddress(AccountData a)
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

        public AccountTypes.Address Merge(AccountTypes.Address src, AccountData update)
        {
            if (update == null) return null;
            if (src == null) return ConvertToAddress(update);

            return new AccountTypes.Address(
                
                update.address == null ? src.address : new FSharpOption<string>(update.address),
                update.postal == null ? src.postal : FSharpOption<int>.Some((int) update.postal),
                update.country == null ? src.country : new FSharpOption<string>(update.country)
            );
        }

        #endregion

        #region ProductData

        //public ProductData Convert(ProductTypes.Product p)
        //{
        //    var metadata = h.OrNull(p.metadata);
        //    MetaData[] meta = null;

        //    if(metadata != null)
        //    {
        //        meta = new MetaData[metadata.Count];

        //        int c = 0;
        //        foreach (var m in metadata)
        //        {
        //            meta[c++] = Convert(m.Value);
        //        }
                
        //    }

        //    return new ProductData {
            
        //        title = p.name,
        //        description = h.OrNull(p.description),
        //        type = p.productType,
        //        price = ConvertToPrice(p),
        //        rating = Convert(p.rating),
        //        owner = p.owner,
        //        meta = meta,
        //        published = p.published
        //    };
        //}

        //public PriceData ConvertToPrice(ProductTypes.Product p)
        //{

        //    return new PriceData {
            
        //        buy = (uint) p.buyPrice,
        //        rent = h.OrNulled(p.rentPrice, price => (uint) price)
        //    };
        //}

        //public RatingData Convert(ProductTypes.Rating r)
        //{
        //    return new RatingData {
            
        //        score = r.rating,
        //        count = (uint) r.votes
        //    };
        //}

        //public MetaData Convert(ProductTypes.Meta m)
        //{
        //    return new MetaData {
            
        //        name = m.key,
        //        value = m.value
        //    };
        //}

        #endregion

        //public PurchaseData Convert()
        //{
        //    return new PurchaseData {


        //    };

        //[DataContract]
//public class PurchaseData
//{
//    [DataMember]
//    public string purchased { get; set; }
//    [DataMember]
//    public uint paid { get; set; }
//    [DataMember]
//    public string type { get; set; }
//    [DataMember]
//    public string expires { get; set; }
//    [DataMember]
//    public string title { get; set; }
//    [DataMember]
//    public uint product { get; set; }
//}
//        }

        public IdData Convert(uint id)
        {
            return new IdData {
            
                id = id
            };
        }
    }
}