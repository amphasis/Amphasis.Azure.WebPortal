using Microsoft.AspNetCore.Mvc;

namespace Amphasis.Azure.Yandex.Models;

public class PaymentConfirmationModel
{
	/// <summary>
	/// Для переводов из кошелька — p2p-incoming. Для переводов с произвольной карты — card-incoming.
	/// </summary>
	[BindProperty(Name = "notification_type")]
	public string NotificationType { get; set; } = "";

	/// <summary>
	/// Идентификатор операции в истории счета получателя
	/// </summary>
	[BindProperty(Name = "operation_id")]
	public string OperationId { get; set; } = "";

	/// <summary>
	/// Сумма, которая зачислена на счет получателя
	/// </summary>
	[BindProperty(Name = "amount")]
	public decimal Amount { get; set; }

	/// <summary>
	/// Сумма, которая списана со счета отправителя
	/// </summary>
	[BindProperty(Name = "withdraw_amount")]
	public decimal WithdrawAmount { get; set; }

	/// <summary>
	/// Код валюты — всегда 643 (рубль РФ согласно ISO 4217)
	/// </summary>
	[BindProperty(Name = "currency")]
	public string Currency { get; set; } = "";

	/// <summary>
	/// Дата и время совершения перевода
	/// </summary>
	[BindProperty(Name = "datetime")]
	public DateTime DateTime { get; set; }

	/// <summary>
	/// Для переводов из кошелька — номер счета отправителя.
	/// Для переводов с произвольной карты — параметр содержит пустую строку
	/// </summary>
	[BindProperty(Name = "sender")]
	public string Sender { get; set; } = "";

	/// <summary>
	/// Для переводов из кошелька <see langword="true"/> означает, что перевод защищен кодом протекции
	/// </summary>
	[BindProperty(Name = "codepro")]
	public bool ProtectionCodeSet { get; set; }

	/// <summary>
	/// Метка платежа. Если ее нет, параметр содержит пустую строку
	/// </summary>
	[BindProperty(Name = "label")]
	public string Label { get; set; } = "";

	/// <summary>
	/// Перевод еще не зачислен
	/// </summary>
	/// <remarks>
	/// Получателю нужно освободить место в кошельке или использовать код протекции (если <see cref="ProtectionCodeSet"/>=<see langword="true"/>)
	/// </remarks>
	[BindProperty(Name = "unaccepted")]
	public bool Unaccepted { get; set; }

	#region Параметры использующиеся для вычисления хэш-кода проверки данных модели

	/// <summary>
	/// SHA-1 hash параметров уведомления
	/// </summary>
	[BindProperty(Name = "sha1_hash")]
	public string Hash { get; set; } = "";

	/// <summary>
	/// Строковое представление суммы, которая зачислена на счет получателя
	/// </summary>
	/// <remarks>
	/// Используется для проверки хэш-кода
	/// </remarks>
	[BindProperty(Name = "amount")]
	public string AmountString { get; set; } = "";

	/// <summary>
	/// Строковое представление признака защиты перевода кодом подтверждения
	/// </summary>
	/// <remarks>
	/// Используется для проверки хэш-кода
	/// </remarks>
	[BindProperty(Name = "codepro")]
	public string ProtectionCodeSetString { get; set; } = "";

	/// <summary>
	/// Строковое представление даты и времени совершения перевода
	/// </summary>
	/// <remarks>
	/// Используется для проверки хэш-кода
	/// </remarks>
	[BindProperty(Name = "datetime")]
	public string DateTimeString { get; set; } = "";

	#endregion Параметры использующиеся для вычисления хэш-кода проверки данных модели

	// Параметры описанные ниже передаются только по HTTPS

	#region ФИО и контакты отправителя перевода (указывает отправитель, если не запрашивались, параметры содержат пустую строку)

	/// <summary>
	/// Фамилия
	/// </summary>
	[BindProperty(Name = "lastname")]
	public string LastName { get; set; } = "";

	/// <summary>
	/// Имя
	/// </summary>
	[BindProperty(Name = "firstname")]
	public string FirstName { get; set; } = "";

	/// <summary>
	/// Отчество
	/// </summary>
	[BindProperty(Name = "fathersname")]
	public string FathersName { get; set; } = "";

	/// <summary>
	/// Адрес электронной почты отправителя перевода
	/// </summary>
	/// <remarks>
	/// Если email не запрашивался, параметр содержит пустую строку
	/// </remarks>
	[BindProperty(Name = "email")]
	public string Email { get; set; } = "";

	/// <summary>
	/// Телефон отправителя перевода
	/// </summary>
	/// <remarks>
	/// Если телефон не запрашивался, параметр содержит пустую строку
	/// </remarks>
	[BindProperty(Name = "phone")]
	public string Phone { get; set; } = "";

	#endregion ФИО и контакты отправителя перевода (указывает отправитель, если не запрашивались, параметры содержат пустую строку)

	#region Адрес доставки (указывает отправитель, если адрес не запрашивался, параметры содержат пустую строку)

	/// <summary>
	/// Город
	/// </summary>
	[BindProperty(Name = "city")]
	public string City { get; set; } = "";

	/// <summary>
	/// Улица
	/// </summary>
	[BindProperty(Name = "street")]
	public string Street { get; set; } = "";

	/// <summary>
	/// Дом
	/// </summary>
	[BindProperty(Name = "building")]
	public string Building { get; set; } = "";

	/// <summary>
	/// Корпус
	/// </summary>
	[BindProperty(Name = "suite")]
	public string Suite { get; set; } = "";

	/// <summary>
	/// Квартира
	/// </summary>
	[BindProperty(Name = "flat")]
	public string Flat { get; set; } = "";

	/// <summary>
	/// Индекс
	/// </summary>
	[BindProperty(Name = "zip")]
	public string Zip { get; set; } = "";

	#endregion Адрес доставки (указывает отправитель, если адрес не запрашивался, параметры содержат пустую строку)
}