﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace Trisome.WPF.Regions
{
    /// <summary>
    /// An entry in an IRegionNavigationJournal representing the URI navigated to.
    /// </summary>
    public class RegionNavigationJournalEntry : IRegionNavigationJournalEntry
    {
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public Uri Uri { get; set; }
        /// <summary>
        /// Gets or sets the NavigationParameters instance.
        /// </summary>
        public IDictionary<string, object> Args { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (Uri != null)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "RegionNavigationJournalEntry:'{0}'",
                    Uri.ToString());
            }

            return base.ToString();
        }
    }
}
