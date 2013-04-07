using System;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

namespace RentIt.Services
{
    /// <summary>
    /// Conversions between the data contract types found in IRentItService and the types used by the backend
    /// </summary>
    public class CoreConverter
    {
        private readonly Helper _h;

        public CoreConverter(Helper helper) {

            _h = helper;
        }

        #region TokenData

        public TokenData Convert(Tuple<string, DateTime> t)
        {
            if (t == null) return null;

            string token = t.Item1;
            string expires = JsonUtility.DateTimeToString(t.Item2);

            return new TokenData { token = token, expires = expires };
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
                name = _h.OrNull(a.info.name),
                address = _h.OrNull(a.info.address.address),
                postal = _h.OrNulled(a.info.address.postal, i => (uint)i),
                country = _h.OrNull(a.info.address.country),
                birth = _h.OrNull(a.info.birth, JsonUtility.DateToString),
                about = _h.OrNull(a.info.about),
                credits = _h.OrNulled(a.info.credits, i => (uint)i),
                created = JsonUtility.DateTimeToString(a.created),
                authenticated = _h.OrNull(authenticated, JsonUtility.DateTimeToString)
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
                update.user ?? src.user,
                update.email ?? src.email,
                update.password == null ? src.password : Account.Password.create(update.password),
                update.created == null ? src.created : JsonUtility.StringToDateTime(update.created),
                update.banned == null ? src.banned : (bool) update.banned,
                Merge(src.info, update),
                update.type ?? src.accType,
                src.version
            );
        }

        public AccountTypes.ExtraAccInfo ConvertToExtra(AccountData a)
        {
            if (a == null) return new AccountTypes.ExtraAccInfo(
                                FSharpOption<string>.None,
                                ConvertToAddress(null),
                                FSharpOption<DateTime>.None,
                                FSharpOption<string>.None,
                                FSharpOption<int>.None
                            );

            return new AccountTypes.ExtraAccInfo(
                new FSharpOption<string>(a.name),
                ConvertToAddress(a),
                FSharpOption<DateTime>.Some(JsonUtility.StringToDate(a.birth)),
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
                update.birth == null ? src.birth : new FSharpOption<DateTime>(JsonUtility.StringToDate(update.birth)),
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
                description = _h.OrNull(p.description),
                type = p.productType,
                price = ConvertToPrice(p),
                rating = Convert(_h.OrNull(p.rating)),
                owner = p.owner,
                meta = Convert(_h.OrNull(p.metadata)),
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
                update.title ?? src.name,
                src.createDate,
                update.type ?? src.productType,
                update.owner ?? src.owner,
                new FSharpOption<ProductTypes.Rating>(Merge(_h.OrNull(src.rating), update.rating)),
                update.published == null ? src.published : (bool) update.published,
                src.id,
                new FSharpOption<FSharpMap<string, ProductTypes.Meta>>(Merge(_h.OrNull(src.metadata), update.meta)),
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
                buy = _h.OrNulled(p.buyPrice, price => (uint)price),
                rent = _h.OrNulled(p.rentPrice, price => (uint)price)
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
            return update == null ? src : Convert(update);
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
                update.name ?? src.key,
                update.value ?? src.value
            );
        }

        public FSharpMap<string, ProductTypes.Meta> Convert(MetaData[] meta)
        {
            if (meta == null || meta.Length == 0) return null;

            var result = MapModule.Empty<string, ProductTypes.Meta>();

            foreach (var m in _h.Map(meta, m => Convert(m)))
            {
                result = result.Add(m.key, m);
            }

            return result;
        }

        public MetaData[] Convert(FSharpMap<string, ProductTypes.Meta> meta)
        {
            if (meta == null || meta.Count == 0) return null;
            
            return _h.Map(meta, pair => Convert(pair.Value));
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

        #region CreditsData

        public PurchaseData Convert(CreditsTypes.RentOrBuy rb)
        {
            CreditsTypes.Transaction t = rb.IsRent 
                                                ? ((CreditsTypes.RentOrBuy.Rent)rb).Item.item 
                                                : ((CreditsTypes.RentOrBuy.Buy)rb).Item.item;
            CreditsTypes.Rent r = rb.IsRent ? ((CreditsTypes.RentOrBuy.Rent) rb).Item : null;

            return new PurchaseData {
                purchased = JsonUtility.DateTimeToString(t.purchased),
                paid = (uint) t.paid,
                expires = rb.IsRent ? JsonUtility.DateTimeToString(r.expires) : null,
                product = (uint) t.product.id,
                type = rb.IsRent ? "R" : "B"
            };
        }
        
        #endregion
        

        public IdData Convert(uint id)
        {
            return new IdData {
                id = id
            };
        }
    }
}