using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROC.Models;
namespace ROC.Data
{
    public class SysDataClassService
    {
        public List<SysDataClass> GetClassList(string typeName)
        {
            using (MyDbContext db = new MyDbContext())
            {
                return db.SysDataClasses.Where(t => t.TypeName.Equals(typeName)).ToList();                
            }
        }

        public SysDataClass Get(Guid id)
        {
            using (MyDbContext db = new MyDbContext())
            {
                return db.SysDataClasses.Find(id);
            }
        }

        public SysDataClass AddClass(string typeName, string name, string parentCode)
        {
            SysDataClass item = new SysDataClass();
            using (MyDbContext db = new MyDbContext())
            {
                var find = db.SysDataClasses.Where(t => t.TypeName.Equals(typeName) && t.ParentCode.Equals(parentCode)).OrderByDescending(t => t.Code).FirstOrDefault();
                if (find == null)
                {
                    item.Id = Guid.NewGuid();
                    item.TypeName = typeName;
                    item.Code = parentCode + "0000";
                    item.Name = name;
                    item.ParentCode = parentCode;
                }
                else
                {
                    item.Id = Guid.NewGuid();
                    item.TypeName = typeName;
                    item.ParentCode = parentCode;
                    item.Name = name;
                    int intCode = int.Parse(find.Code.Substring(parentCode.Length));
                    intCode++;
                    item.Code = parentCode + intCode.ToString().PadLeft(4, '0');
                }
                var  model=db.SysDataClasses.Add(item);
                db.SaveChanges();
                return model;
            }
        }

        public SysDataClass Save(SysDataClass model)
        {
            using (MyDbContext db = new MyDbContext())
            {
                var m = db.SysDataClasses.Find(model.Id);
                if(m!=null)
                {
                    m.Name = model.Name;
                }
                else
                {
                    db.SysDataClasses.Add(model);
                }
                db.SaveChanges();
                return m;
            }
        }
        public bool Delete(Guid id)
        {
            using (MyDbContext db = new MyDbContext())
            {
                var m = db.SysDataClasses.Find(id);
                if(m!=null)
                {
                    db.SysDataClasses.Remove(m);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
    }

}
