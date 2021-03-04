using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFunctions;

namespace SchoolRecords.Model
{
    public record Student
    {
        [Hidden]
        public virtual int Id { get; init; }

        [MemberOrder(1)]
        public virtual string StudentNumber { get; init; }

        [MemberOrder(2)]
        public virtual string FirstName { get; init; }

        [MemberOrder(3)]
        public virtual string LastName { get; init; }

        [Hidden]
        public virtual DateTime DateOfBirth { get; init; }

        [MemberOrder(4)]
        public virtual Teacher Tutor { get; init; }

        [MemberOrder(5)]
        [RenderEagerly]
        [TableView(false, "Subject", "SetName", "Teacher")]
        public virtual ICollection<TeachingSet> Sets { get; init; } = new List<TeachingSet>();

        public override string ToString() => $"{FirstName} {LastName}, Age {Student_Functions.Age(this)}";

        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(Student other) => ReferenceEquals(this, other);
    }
}
