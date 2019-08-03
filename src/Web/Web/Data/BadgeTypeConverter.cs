using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CodeMooc.Web.Data {

    public class BadgeTypeConverter {

        private static readonly IDictionary<BadgeType, string> _mapToProvider;
        private static readonly IDictionary<string, BadgeType> _mapToModel;

        static BadgeTypeConverter() {
            _mapToProvider = new Dictionary<BadgeType, string> {
                { BadgeType.Member, "Iscrizione" },
                { BadgeType.Patron, "Sostenitore" },
                { BadgeType.GoldPatron, "SostenitoreGold" },
                { BadgeType.Sponsor, "DonatoreSponsor" }
            };

            _mapToModel = new Dictionary<string, BadgeType>(from p in _mapToProvider.Keys
                                                            select new KeyValuePair<string, BadgeType>(_mapToProvider[p], p));
        }

        public static ValueConverter Create() {
            return new ValueConverter<BadgeType, string>(
                input => _mapToProvider[input],
                input => _mapToModel[input]
            );
        }

    }

}
