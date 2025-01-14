﻿using Application.Contracts;
using Application.Features.Author.Commands.Create.Event.Email;
using Application.Helper.Constants;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using SharedKernel.Resources.API_Helper;
using SharedKernel.Resources.CQRS;
using SharedKernel.Resources.Logging;

namespace Application.Features.Author.Commands.Create;

public class CreateAuthorCommandHandler: ICommandHandler<CreateAuthorCommand>
{
    private readonly IWesterosContext _context;
    private readonly IPublisher _publisher;
    public CreateAuthorCommandHandler(IWesterosContext context, IMediator mediator)
    {
        _context = context;
        _publisher = mediator;
    }

    public async Task<ApiResponse> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validate = new CreateAuthorCommandValidator();
            await validate.ValidateAndThrowAsync(request, cancellationToken);

            await CreateAuthorBuilder(request, cancellationToken);
            await _publisher.Publish(new AuthorCreatedEmailEvent(Guid.NewGuid(), "sample email"), cancellationToken);
            return ApiResponse.GetSuccess(null, null,BaseConstant.Created);
        }
        catch (Exception ex)
        {
            WesterosLogger.LogError(ErrorConstant.DATABASE_CREATE_ERROR, ex.Message);
            return ApiResponse.GetFailed(null, null, ex.Message);

        }

    }

    private async Task CreateAuthorBuilder(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var data = new Domain.Models.Authors.Author();
            data.FirstName = request.FirstName;
            data.LastName = request.LastName;
            data.Email = request.Email;
            data.Phone = request.Phone;
            data.Address = request.GetAddress();

            await _context.Author.AddAsync(data, cancellationToken);
            await _context.SaveChangesAsync();
        }
        catch (SqlException ex)
        {
            WesterosLogger.LogError(ErrorConstant.DATABASE_CREATE_ERROR, ex.Message);
        }
    }
}