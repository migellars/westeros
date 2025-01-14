﻿using Application.Dto;
using SharedKernel.Resources.CQRS;

namespace Application.Features.Lesson.Queries.GetById;

public class GetLessonByIdQuery : IQuery<TutorialDto>
{
    public GetLessonByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}