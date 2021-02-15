﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Repositories
{

    public class TaskRepository<TDocument> : ITaskRepository<TDocument>
        where TDocument : IDocument
    {
        //private readonly TaskManagerContext _context;
        //public TaskRepository(TaskManagerContext context)
        //{
        //    _context = context;

        //}

        //public TaskModel Get(string Id)
        //    => _context.Tasks.SingleOrDefault(x => x.Id == Id);

        //public IQueryable<TaskModel> GetAllActive()
        //    => _context.Tasks.Where(x => !x.Done);

        //public void Add(TaskModel task)
        //{
        //    _context.Tasks.Add(task);
        //    _context.SaveChanges();
        //}

        //public void Update(string Id, TaskModel task)
        //{
        //    var result = _context.Tasks.SingleOrDefault(x => x.Id == Id);
        //    if (result != null)
        //    {
        //        result.Name = task.Name;
        //        result.Description = task.Description;
        //        result.Done = task.Done;

        //        _context.SaveChanges();
        //    }
        //}

        //public void Delete(string Id)
        //{
        //    var result = _context.Tasks.SingleOrDefault(x => x.Id == Id);
        //    if (result != null)
        //    {
        //        _context.Tasks.Remove(result);
        //        _context.SaveChanges();
        //    }
        //}
        private readonly IMongoCollection<TDocument> _collection;

        public TaskRepository(IDatabaseSettings settings)
        {
            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }

        public virtual IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public virtual IEnumerable<TDocument> FilterBy(
            Expression<Func<TDocument, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).ToEnumerable();
        }

        public virtual IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        public virtual TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).FirstOrDefault();
        }

        public virtual Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
        }

        public virtual TDocument FindById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            return _collection.Find(filter).SingleOrDefault();
        }

        public virtual Task<TDocument> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
                return _collection.Find(filter).SingleOrDefaultAsync();
            });
        }
        public virtual IEnumerable<TaskModel> FindAllActiveTasks()
        {
            return (_collection as IMongoCollection<TaskModel>).Find(x => !x.Done).ToEnumerable();
        }

        public virtual void InsertOne(TDocument document)
        {
            _collection.InsertOne(document);
        }

        public virtual Task InsertOneAsync(TDocument document)
        {
            return Task.Run(() => _collection.InsertOneAsync(document));
        }

        public void InsertMany(ICollection<TDocument> documents)
        {
            _collection.InsertMany(documents);
        }


        public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
        {
            await _collection.InsertManyAsync(documents);
        }
        public virtual void UpdateTask(string id, TaskModel task)
        {
            //var result = FindById(id);
            //if (result != null && result is TaskModel item)
            //{
            //    item.Name = task.Name;
            //    item.Description = task.Description;
            //    item.Done = task.Done;
            //}
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, new ObjectId(id));
            var update = Builders<TDocument>.Update
                .Set("Name", task.Name)
                .Set("Description", task.Description)
                .Set("Done", task.Done);
            _collection.UpdateOne(filter, update);
        }
        public void ReplaceOne(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            _collection.FindOneAndReplace(filter, document);
        }

        public virtual async Task ReplaceOneAsync(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document);
        }

        public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            _collection.FindOneAndDelete(filterExpression);
        }

        public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
        }

        public void DeleteById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            _collection.FindOneAndDelete(filter);
        }

        public Task DeleteByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
                _collection.FindOneAndDeleteAsync(filter);
            });
        }

        public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
        {
            _collection.DeleteMany(filterExpression);
        }

        public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            return Task.Run(() => _collection.DeleteManyAsync(filterExpression));
        }
    }
}