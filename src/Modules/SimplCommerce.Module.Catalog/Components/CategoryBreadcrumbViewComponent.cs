﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.ViewModels;

namespace SimplCommerce.Module.Catalog.Components
{
    public class CategoryBreadcrumbViewComponent : ViewComponent
    {
        private readonly IRepository<Category> categoryRepository;

        public CategoryBreadcrumbViewComponent(IRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public IViewComponentResult Invoke(long? categoryId, IEnumerable<long> categoryIds)
        {
            IList<BreadcrumbViewModel> breadcrumbs = new List<BreadcrumbViewModel>();
            if (categoryId.HasValue)
            {
                breadcrumbs = Create(categoryId.Value);
            }
            else
            {
                var breadcrumbList = categoryIds.Select(Create).ToList();
                breadcrumbs = breadcrumbList.OrderByDescending(x => x.Count).First();
            }

            return View("/Modules/SimplCommerce.Module.Catalog/Views/Components/CategoryBreadcrumb.cshtml", breadcrumbs);
        }

        private IList<BreadcrumbViewModel> Create(long categoryId)
        {
            var category = categoryRepository
                .Query()
                .Include(x => x.Parent)
                .FirstOrDefault(x => x.Id == categoryId);
            var breadcrumbModels = new List<BreadcrumbViewModel>
            {
                new BreadcrumbViewModel
                {
                    Text = category.Name,
                    Url = category.SeoTitle
                }
            };
            var parentCategory = category.Parent;
            while (parentCategory != null)
            {
                breadcrumbModels.Insert(0, new BreadcrumbViewModel
                {
                    Text = parentCategory.Name,
                    Url = parentCategory.SeoTitle
                });
                parentCategory = parentCategory.Parent;
            }

            return breadcrumbModels;
        }
    }
}
