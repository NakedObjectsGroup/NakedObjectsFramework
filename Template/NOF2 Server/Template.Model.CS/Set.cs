using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFramework;
using NakedObjects;

namespace Template.Model
{
    public class Set
    {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        [Hidden]
        public virtual int Id { get; set; }

        [MemberOrder(1)]
        public virtual string SetName { get; set; }

        [MemberOrder(2)]
        public virtual Subject Subject { get; set; }

        [MemberOrder(3), Range(9,13)]
        public virtual int YearGroup { get; set; }

        [MemberOrder(4)]
        public virtual Teacher Teacher { get; set; }


        [Eagerly(Do.Rendering)]
        [TableView(false, "FullName")]
        [MemberOrder(5)]
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();

        public void AddStudentToSet(Student student) => Students.Add(student);

        public void RemoveStudentFromSet(Student student) => Students.Remove(student);

        public IList<Student> Choices0RemoveStudentFromSet() => Students.ToList();

        public void TransferStudentTo(Student student, Set newSet)
        {
            RemoveStudentFromSet(student);
            newSet.AddStudentToSet(student);
        }

        public IList<Student> Choices0TransferStudentTo() => Students.ToList();

        [PageSize(10)]
        public IList<Set> Choices1TransferStudentTo()
        {
            int subjId = Subject.Id;
            int yg = YearGroup;
            return Container.Instances<Set>().Where(s => s.Subject.Id == subjId && s.YearGroup == yg).ToList();
        }

        public override string ToString() => SetName;
    }
}

