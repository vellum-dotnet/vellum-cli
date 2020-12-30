// <copyright file="ContentFragment.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Abstractions.Content
{
    using System;
    using System.Collections.Generic;

    public class ContentFragment : IEquatable<ContentFragment>, IContent
    {
        public int Position { get; set; }

        public string Id { get; set; }

        public string ContentType { get; set; }

        public Dictionary<string, dynamic> MetaData { get; set; } = new Dictionary<string, dynamic>();

        public string Hash { get; set; }

        public string Body { get; set; }

        public DateTime Date { get; set; }

        public PublicationStatus PublicationStatus { get; set; }

        public static bool operator ==(ContentFragment left, ContentFragment right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ContentFragment left, ContentFragment right)
        {
            return !Equals(left, right);
        }

        public bool Equals(ContentFragment other)
        {
            if (other is null)
            {
                return false;
            }

            return ReferenceEquals(this, other) || (this.ContentType == other.ContentType && this.Body == other.Body);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((ContentFragment)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.ContentType?.GetHashCode() ?? 0) * 397) ^ (this.Body?.GetHashCode() ?? 0);
            }
        }
    }
}