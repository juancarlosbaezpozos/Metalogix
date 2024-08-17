using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Taxonomy
{
	public abstract class SPTermSetItem : SPTaxonomyItem, IComparable<SPTermSetItem>, IEquatable<SPTermSetItem>
	{
		private SPTermCollection _terms = null;

		private readonly object _termsLock = new object();

		internal abstract SPConnection Connection
		{
			get;
		}

		public abstract bool IsAvailableForTagging
		{
			get;
		}

		public abstract string Owner
		{
			get;
		}

		public virtual SPTermCollection Terms
		{
			get
			{
				lock (this._termsLock)
				{
					if (this._terms == null)
					{
						SPTermCollection sPTermCollection = new SPTermCollection(this);
						sPTermCollection.FetchData();
						this._terms = sPTermCollection;
					}
				}
				return this._terms;
			}
			internal set
			{
				lock (this._termsLock)
				{
					this._terms = value;
				}
			}
		}

		protected SPTermSetItem()
		{
		}

		public virtual int CompareTo(SPTermSetItem other)
		{
			int num;
			num = (other != null ? this.Id.CompareTo(other.Id) : 1);
			return num;
		}

		public virtual bool Equals(SPTermSetItem other)
		{
			return (other == null ? false : this.Id.Equals(other.Id));
		}

		public virtual void FetchData(bool refetchTerms)
		{
			lock (this._termsLock)
			{
				if (this._terms == null)
				{
					this._terms = new SPTermCollection(this);
				}
				else
				{
					this._terms.Clear();
				}
				if (refetchTerms)
				{
					this._terms.FetchData();
				}
			}
		}

		public SPTerm GetTerm(string termName)
		{
			return this.Terms[termName];
		}

		internal abstract string GetTermCollectionXml();
	}
}