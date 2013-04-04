using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

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

        public ProductData Convert(ProductTypes.Product p)
        {
            return new ProductData
            {

                title = p.name,
                description = h.OrNull(p.description),
                type = p.productType,
                price = ConvertToPrice(p),
                rating = Convert(h.OrNull(p.rating)),
                owner = p.owner,
                meta = Convert(h.OrNull(p.metadata)),
                published = p.published
            };
        }

        public ProductTypes.Product Convert(ProductData p)
        {
            if (p == null) return null;

            // Make a dummy product with all hidden values having their default values, and all required fields nulled out
            var dummy = Product.make("dummy", "dummy", "dummy", FSharpOption<string>.None, FSharpOption<int>.None, FSharpOption<int>.None);
            dummy = new ProductTypes.Product(null, dummy.createDate, null, null, dummy.rating, dummy.published, dummy.id, dummy.metadata, dummy.description, dummy.rentPrice, dummy.buyPrice);

            return Merge(dummy, p);
        }

        public ProductTypes.Product Merge(ProductTypes.Product src, ProductData update)
        {
            if (update == null) return src;
            if (src == null) return Convert(update);

            return new ProductTypes.Product
            (
                update.title == null ? src.name : update.title,
                src.createDate,
                update.type == null ? src.productType : update.type,
                update.owner == null ? src.owner : update.owner,
                new FSharpOption<ProductTypes.Rating>(Merge(h.OrNull(src.rating), update.rating)),
                update.published == null ? src.published : (bool) update.published,
                src.id,
                new FSharpOption<FSharpMap<string, ProductTypes.Meta>>(Merge(h.OrNull(src.metadata), update.meta)),
                update.description == null ? src.description : new FSharpOption<string>(update.description),
                update.price == null || update.price.rent == null ? src.rentPrice : FSharpOption<int>.Some((int) update.price.rent),
                update.price == null || update.price.buy == null ? src.buyPrice : FSharpOption<int>.Some((int) update.price.buy)
            );
        }

        public PriceData ConvertToPrice(ProductTypes.Product p)
        {
            if (p == null) return null;

            return new PriceData
            {

                buy = h.OrNulled(p.buyPrice, price => (uint)price),
                rent = h.OrNulled(p.rentPrice, price => (uint)price)
            };
        }

        public RatingData Convert(ProductTypes.Rating r)
        {
            if (r == null) return new RatingData
            {
                score = 0,
                count = 0
            };

            return new RatingData
            {
                score = r.rating,
                count = (uint)r.votes
            };
        }

        public ProductTypes.Rating Convert(RatingData r)
        {
            if (r == null) return new ProductTypes.Rating
            (
                0,
                0
            );

            return new ProductTypes.Rating
            (
                r.score,
                (int)r.count
            );
        }

        public ProductTypes.Rating Merge(ProductTypes.Rating src, RatingData update)
        {
            if (update == null) return src;
            else return Convert(update);
        }

        public MetaData Convert(ProductTypes.Meta m)
        {
            if (m == null) return null;

            return new MetaData
            {
                name = m.key,
                value = m.value
            };
        }

        public ProductTypes.Meta Convert(MetaData m)
        {
            if (m == null) return null;

            return new ProductTypes.Meta
            (
                m.name,
                m.value
            );
        }

        public ProductTypes.Meta Merge(ProductTypes.Meta src, MetaData update)
        {
            if (update == null) return src;
            if (src == null) return Convert(update);

            return new ProductTypes.Meta
            (
                update.name == null ? src.key : update.name,
                update.value == null ? src.value : update.value
            );
        }

        public FSharpMap<string, ProductTypes.Meta> Convert(MetaData[] meta)
        {
            if (meta == null || meta.Length == 0) return null;

            var result = MapModule.Empty<string, ProductTypes.Meta>();

            foreach (var m in h.Map(meta, m => Convert(m)))
            {
                result = result.Add(m.key, m);
            }

            return result;
        }

        public MetaData[] Convert(FSharpMap<string, ProductTypes.Meta> meta)
        {
            if (meta == null || meta.Count == null) return null;

            return h.Map(meta, pair => Convert(pair.Value));
        }

        public FSharpMap<string, ProductTypes.Meta> Merge(FSharpMap<string, ProductTypes.Meta> src, MetaData[] update)
        {
            if (src == null) return Convert(update);
            if (update == null) return src;

            foreach (MetaData m in update)
            {
                if(m == null) continue;
                if (src.ContainsKey(m.name))
                {

                    var converted = Merge(src[m.name], m);
                    src = src.Add(converted.key, converted);
                }
                else src = src.Add(m.name, Convert(m));
            }

            return src;
        }

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