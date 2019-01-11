using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using CodeMooc.Web.Resources;

namespace CodeMooc.Web {

    public static class BadgeTypeExtensions {

        private static readonly Dictionary<string, BadgeType> _routeNames = new Dictionary<string, BadgeType> {
            { "socio2019", BadgeType.Member2019 },
            { "sostenitore2019", BadgeType.Patron2019 },
            { "sostenitoregold2019", BadgeType.GoldPatron2019 },
            { "sponsor2019", BadgeType.Sponsor2019 },
        };

        private static readonly int[] _widths = new int[] {
            250,
            640,
            1000
        };

        public static bool TryParseBadgeType(this string s, out BadgeType badgeType) {
            var key = s.Trim().ToLowerInvariant();
            if (_routeNames.ContainsKey(key)) {
                badgeType = _routeNames[key];
                return true;
            }
            else {
                badgeType = default(BadgeType);
                return false;
            }
        }

        public static string GetSrcSet(this BadgeType badgeType) {
            return string.Join(", ", from width in _widths
                                     orderby width
                                     select string.Format("/static/{0}-{1}.png {1}w", badgeType.GetRootPath(), width));
        }

        public static string GetRootPath(this BadgeType badgeType) {
            switch(badgeType) {
                default:
                case BadgeType.Member2019:
                    return "badges/2019/member";
                case BadgeType.Patron2019:
                    return "badges/2019/patron";
                case BadgeType.GoldPatron2019:
                    return "badges/2019/patron-gold";
                case BadgeType.Sponsor2019:
                    return "badges/2019/sponsor";
            }
        }

        public static string GetName(this BadgeType badgeType) {
            return BadgeNames.ResourceManager.GetString(badgeType.ToString());
        }

        public static string GetDescription(this BadgeType badgeType) {
            return BadgeDescriptions.ResourceManager.GetString(badgeType.ToString());
        }

        public static string GetEvidence(this BadgeType badgeType) {
            return BadgeEvidenceDescriptions.ResourceManager.GetString(badgeType.ToString());
        }

    }

}
