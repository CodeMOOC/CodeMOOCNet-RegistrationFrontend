using System.Collections.Generic;
using System.Linq;
using CodeMooc.Web.Resources;

namespace CodeMooc.Web {

    public static class BadgeTypeExtensions {

        private static readonly Dictionary<string, BadgeType> _legacyRouteNames = new Dictionary<string, BadgeType> {
            { "socio2019", BadgeType.Member },
            { "sostenitore2019", BadgeType.Patron },
            { "sostenitoregold2019", BadgeType.GoldPatron },
            { "sponsor2019", BadgeType.Sponsor },
        };

        private static readonly Dictionary<string, BadgeType> _routeNames = new Dictionary<string, BadgeType> {
            { "socio", BadgeType.Member },
            { "sostenitore", BadgeType.Patron },
            { "sostenitoregold", BadgeType.GoldPatron },
            { "sponsor", BadgeType.Sponsor },
        };

        private static readonly Dictionary<BadgeType, string> _reverseNames;

        static BadgeTypeExtensions() {
            _reverseNames = new Dictionary<BadgeType, string>(from p in _routeNames.Keys
                                                              select new KeyValuePair<BadgeType, string>(_routeNames[p], p));
        }

        private static readonly int[] _widths = new int[] {
            250,
            640,
            1000
        };

        public static bool TryParseLegacyBadgeType(this string s, out BadgeType badgeType) {
            return TryParseBadgeTypeInternal(_legacyRouteNames, s, out badgeType);
        }

        public static bool TryParseBadgeType(this string s, out BadgeType badgeType) {
            return TryParseBadgeTypeInternal(_routeNames, s, out badgeType);
        }

        private static bool TryParseBadgeTypeInternal(IDictionary<string, BadgeType> map, string s, out BadgeType badgeType) {
            var key = s.Trim().ToLowerInvariant();
            if(map.ContainsKey(key)) {
                badgeType = map[key];
                return true;
            }
            else {
                badgeType = default;
                return false;
            }
        }

        public static string GetSrcSet(this BadgeType badgeType, int year) {
            return string.Join(", ", from width in _widths
                                     orderby width
                                     select string.Format("{0}-{1}.png {1}w", badgeType.GetRootPath(year), width));
        }

        public static string GetRootPath(this BadgeType badgeType, int year) {
            switch(badgeType) {
                default:
                case BadgeType.Member:
                    return $"/badges/{year}/member";
                case BadgeType.Patron:
                    return $"/badges/{year}/patron";
                case BadgeType.GoldPatron:
                    return $"/badges/{year}/patron-gold";
                case BadgeType.Sponsor:
                    return $"/badges/{year}/sponsor";
            }
        }

        public static string GetName(this BadgeType badgeType, int year) {
            return BadgeNames.ResourceManager.GetString($"{badgeType}{year}");
        }

        public static string GetDescription(this BadgeType badgeType, int year) {
            return BadgeDescriptions.ResourceManager.GetString($"{badgeType}{year}");
        }

        public static string GetEvidence(this BadgeType badgeType, int year) {
            return BadgeEvidenceDescriptions.ResourceManager.GetString($"{badgeType}{year}");
        }

        public static string GetPathToken(this BadgeType badgeType) {
            return _reverseNames[badgeType];
        }

    }

}
